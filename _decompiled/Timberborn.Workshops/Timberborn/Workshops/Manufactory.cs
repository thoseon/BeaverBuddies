using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.MechanicalSystem;
using Timberborn.Persistence;
using Timberborn.ResourceCountingSystem;
using Timberborn.ScienceSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.Workshops;

public class Manufactory : BaseComponent, IAwakableComponent, IPersistentEntity, IFinishedStateListener, IRegisteredComponent, IRecipeSelector, IDuplicable<Manufactory>, IDuplicable, IGoodProcessor
{
	private static readonly ComponentKey ManufactoryKey = new ComponentKey("Manufactory");

	private static readonly PropertyKey<string> CurrentRecipeIdKey = new PropertyKey<string>("CurrentRecipe");

	private static readonly PropertyKey<float> ProductionProgressKey = new PropertyKey<float>("ProductionProgress");

	private static readonly PropertyKey<float> FuelRemainingKey = new PropertyKey<float>("FuelRemaining");

	private static readonly PropertyKey<bool> IngredientsConsumedKey = new PropertyKey<bool>("IngredientsConsumed");

	private readonly ScienceService _scienceService;

	private readonly RecipeSpecService _recipeSpecService;

	private MechanicalNode _mechanicalNode;

	private RecipeGoodDisallower _recipeGoodDisallower;

	private readonly List<IManufactoryLimiter> _manufactoryLimiters = new List<IManufactoryLimiter>();

	private ManufactorySpec _manufactorySpec;

	private bool _ingredientsConsumed;

	private readonly List<GoodAmount> _processedGoods = new List<GoodAmount>();

	public Inventory Inventory { get; private set; }

	public float FuelRemaining { get; private set; }

	public float ProductionProgress { get; private set; }

	public RecipeSpec CurrentRecipe { get; private set; }

	public ImmutableArray<RecipeSpec> ProductionRecipes { get; private set; }

	public ReadOnlyList<GoodAmount> ProcessedGoods => _processedGoods.AsReadOnlyList();

	public bool HasFuel
	{
		get
		{
			if (CurrentRecipe.ConsumesFuel && Inventory.UnreservedAmountInStock(CurrentRecipe.Fuel) <= 0)
			{
				return FuelRemaining > 0f;
			}
			return true;
		}
	}

	public bool IsReadyToProduce
	{
		get
		{
			if (HasCurrentRecipe && HasAllIngredients && HasUnreservedCapacityForCurrentProducts() && HasFuel && HasPower)
			{
				return ProductionEfficiency() > 0f;
			}
			return false;
		}
	}

	public bool HasCurrentRecipe => CurrentRecipe != null;

	public bool NeedsInventory
	{
		get
		{
			if (!ProductionRecipes.Any((RecipeSpec recipe) => recipe.ConsumesFuel) && !ProductionRecipes.Any((RecipeSpec recipe) => recipe.ConsumesIngredients))
			{
				return ProductionRecipes.Any((RecipeSpec recipe) => recipe.ProducesProducts);
			}
			return true;
		}
	}

	public bool HasAllIngredients
	{
		get
		{
			if (!HasAllIngredientsInternal())
			{
				return _ingredientsConsumed;
			}
			return true;
		}
	}

	private bool HasPower
	{
		get
		{
			if (NeedsPower)
			{
				return _mechanicalNode.ActiveAndPowered;
			}
			return true;
		}
	}

	private bool NeedsPower
	{
		get
		{
			if ((bool)_mechanicalNode)
			{
				return _mechanicalNode.IsConsumer;
			}
			return false;
		}
	}

	public event EventHandler<ProductionProgressedEventArgs> ProductionProgressed;

	public event EventHandler ProductionFinished;

	public event EventHandler RecipeChanged;

	public Manufactory(ScienceService scienceService, RecipeSpecService recipeSpecService)
	{
		_scienceService = scienceService;
		_recipeSpecService = recipeSpecService;
	}

	public void Awake()
	{
		_mechanicalNode = GetComponent<MechanicalNode>();
		_recipeGoodDisallower = GetComponent<RecipeGoodDisallower>();
		GetComponents(_manufactoryLimiters);
		_manufactorySpec = GetComponent<ManufactorySpec>();
		InitializeRecipes();
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		if (NeedsInventory)
		{
			Inventory.Enable();
			_recipeGoodDisallower.UpdateAllowedAmounts(CurrentRecipe);
		}
	}

	public void OnExitFinishedState()
	{
		if (NeedsInventory)
		{
			Inventory.Disable();
		}
		DisableComponent();
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(ManufactoryKey);
		if (ProductionRecipes.Length > 1 && HasCurrentRecipe)
		{
			component.Set(CurrentRecipeIdKey, CurrentRecipe.Id);
		}
		if (FuelRemaining > 0f)
		{
			component.Set(FuelRemainingKey, FuelRemaining);
		}
		component.Set(ProductionProgressKey, ProductionProgress);
		component.Set(IngredientsConsumedKey, _ingredientsConsumed);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ManufactoryKey);
		if (component.Has(CurrentRecipeIdKey))
		{
			string recipeId = component.Get(CurrentRecipeIdKey);
			RecipeSpec recipeSpec = ProductionRecipes.SingleOrDefault((RecipeSpec recipe) => recipe.Id == recipeId) ?? ProductionRecipes.SingleOrDefault((RecipeSpec recipe) => recipe.BackwardCompatibleIds.Contains(recipeId));
			if (recipeSpec != null)
			{
				CurrentRecipe = recipeSpec;
			}
		}
		if (component.Has(FuelRemainingKey))
		{
			FuelRemaining = component.Get(FuelRemainingKey);
		}
		ProductionProgress = component.Get(ProductionProgressKey);
		_ingredientsConsumed = component.Has(IngredientsConsumedKey) && component.Get(IngredientsConsumedKey);
		UpdateGoodRegistry();
	}

	public void DuplicateFrom(Manufactory source)
	{
		RecipeSpec currentRecipe = source.CurrentRecipe;
		if (CurrentRecipe != currentRecipe && ((currentRecipe == null && ProductionRecipes.Length > 1) || ProductionRecipes.Contains(currentRecipe)))
		{
			SetRecipe(currentRecipe);
		}
	}

	public void InitializeInventory(Inventory inventory)
	{
		Asserts.FieldIsNull(this, Inventory, "Inventory");
		Inventory = inventory;
	}

	public void SetRecipe(RecipeSpec selectedRecipe)
	{
		CurrentRecipe = selectedRecipe;
		ResetProductionProgress();
		if (base.Enabled)
		{
			_recipeGoodDisallower.UpdateAllowedAmounts(CurrentRecipe);
		}
		RecipeChanged?.Invoke(this, EventArgs.Empty);
	}

	public void IncreaseProductionProgress(float workedHours)
	{
		if (IsReadyToProduce)
		{
			float num = workedHours / CurrentRecipe.CycleDurationInHours * ProductionEfficiency();
			float num2 = Mathf.Min(num, MaxProductionProgressChange(num));
			RefillFuelAndTakeIngredients(num2);
			ProductionProgress += num2;
			ProductionProgressed?.Invoke(this, new ProductionProgressedEventArgs(num2));
			if (ProductionProgress >= 1f)
			{
				FinishProduction();
			}
		}
	}

	public void ResetProductionProgress()
	{
		_ingredientsConsumed = false;
		ProductionProgress = 0f;
		UpdateGoodRegistry();
		ProductionProgressed?.Invoke(this, new ProductionProgressedEventArgs(0f));
	}

	public bool HasUnreservedCapacityForCurrentProducts()
	{
		for (int i = 0; i < CurrentRecipe.Products.Length; i++)
		{
			GoodAmountSpec goodAmountSpec = CurrentRecipe.Products[i];
			if (!Inventory.HasUnreservedCapacity(goodAmountSpec.ToGoodAmount()))
			{
				return false;
			}
		}
		return true;
	}

	private void UpdateGoodRegistry()
	{
		_processedGoods.Clear();
		if (!HasCurrentRecipe)
		{
			return;
		}
		if (_ingredientsConsumed)
		{
			foreach (GoodAmountSpec ingredient in CurrentRecipe.Ingredients)
			{
				_processedGoods.Add(ingredient.ToGoodAmount());
			}
		}
		if (CurrentRecipe.ConsumesFuel && FuelRemaining > 0f)
		{
			_processedGoods.Add(new GoodAmount(CurrentRecipe.Fuel, 1));
		}
	}

	private void InitializeRecipes()
	{
		ProductionRecipes = (from id in _manufactorySpec.ProductionRecipeIds
			select _recipeSpecService.GetRecipe(id) into recipe
			where recipe.Blueprint.IsAllowedByFeatureToggles()
			select recipe).ToImmutableArray();
		if (ProductionRecipes.Length == 1)
		{
			SetRecipe(ProductionRecipes[0]);
		}
	}

	private void RefillFuelAndTakeIngredients(float productionProgressChange)
	{
		if (CurrentRecipe.ConsumesFuel && FuelRemaining <= 0f)
		{
			Inventory.TakeConsumed(new GoodAmount(CurrentRecipe.Fuel, 1));
			FuelRemaining = 1f;
		}
		if (productionProgressChange > 0f && !_ingredientsConsumed)
		{
			_ingredientsConsumed = true;
			foreach (GoodAmountSpec ingredient in CurrentRecipe.Ingredients)
			{
				Inventory.TakeConsumed(ingredient.ToGoodAmount());
			}
			UpdateGoodRegistry();
		}
	}

	private float MaxProductionProgressChange(float expectedProductionProgressChange)
	{
		float num = 1f;
		for (int i = 0; i < _manufactoryLimiters.Count; i++)
		{
			float val = _manufactoryLimiters[i].MaxProductionProgressChange(expectedProductionProgressChange);
			num = Math.Min(num, val);
		}
		return num;
	}

	private float ProductionEfficiency()
	{
		float num = (NeedsPower ? _mechanicalNode.PowerEfficiency : 1f);
		for (int i = 0; i < _manufactoryLimiters.Count; i++)
		{
			num = Math.Min(num, _manufactoryLimiters[i].ProductionEfficiency());
		}
		return num;
	}

	private void FinishProduction()
	{
		foreach (GoodAmountSpec product in CurrentRecipe.Products)
		{
			Inventory.GiveProduced(product.ToGoodAmount());
		}
		_scienceService.AddPoints(CurrentRecipe.ProducedSciencePoints);
		ConsumeFuel();
		ResetProductionProgress();
		ProductionFinished?.Invoke(this, EventArgs.Empty);
	}

	private void ConsumeFuel()
	{
		if (CurrentRecipe.ConsumesFuel)
		{
			float num = 1f / (float)CurrentRecipe.CyclesFuelLasts;
			FuelRemaining -= num;
			if (FuelRemaining < num * 0.5f)
			{
				FuelRemaining = 0f;
			}
		}
	}

	private bool HasAllIngredientsInternal()
	{
		for (int i = 0; i < CurrentRecipe.Ingredients.Length; i++)
		{
			GoodAmountSpec goodAmountSpec = CurrentRecipe.Ingredients[i];
			if (!Inventory.HasUnreservedStock(goodAmountSpec.ToGoodAmount()))
			{
				return false;
			}
		}
		return true;
	}
}
