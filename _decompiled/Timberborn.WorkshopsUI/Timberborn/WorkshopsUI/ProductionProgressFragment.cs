using System;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using Timberborn.WorkSystem;
using Timberborn.Workshops;
using UnityEngine.UIElements;

namespace Timberborn.WorkshopsUI;

public class ProductionProgressFragment : IEntityPanelFragment
{
	private static readonly string NoRecipeClass = "production-progress-fragment__no-recipe";

	private static readonly string FuelRemainingLocKey = "Work.FuelRemaining";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private Manufactory _manufactory;

	private Workplace _workplace;

	private ManufactoryDescriber _manufactoryDescriber;

	private VisualElement _root;

	private Label _progressText;

	private Label _craftingTime;

	private Label _fuelRemaining;

	private VisualElement _input;

	private VisualElement _output;

	private bool _enabled;

	private readonly Phrase _productionProgressPhrase = Phrase.New().FormatPercentFloored();

	public ProductionProgressFragment(VisualElementLoader visualElementLoader, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/ProductionProgressFragment");
		_progressText = _root.Q<Label>("ProgressText");
		_craftingTime = _root.Q<Label>("CraftingTime");
		_fuelRemaining = _root.Q<Label>("FuelRemaining");
		_input = _root.Q<VisualElement>("InputWrapper");
		_output = _root.Q<VisualElement>("OutputWrapper");
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_manufactory = entity.GetComponent<Manufactory>();
		if ((bool)_manufactory)
		{
			_workplace = entity.GetComponent<Workplace>();
			_manufactoryDescriber = entity.GetComponent<ManufactoryDescriber>();
			if (_manufactory.HasCurrentRecipe)
			{
				AddProductionItem();
				UpdateCraftingTime();
			}
			_enabled = true;
			_manufactory.RecipeChanged += OnProductionRecipeChanged;
			if ((bool)_workplace)
			{
				_workplace.WorkerAssigned += OnWorkerChanged;
				_workplace.WorkerUnassigned += OnWorkerChanged;
			}
		}
	}

	public void ClearFragment()
	{
		if ((bool)_manufactory)
		{
			_manufactory.RecipeChanged -= OnProductionRecipeChanged;
			if ((bool)_workplace)
			{
				_workplace.WorkerAssigned -= OnWorkerChanged;
				_workplace.WorkerUnassigned -= OnWorkerChanged;
			}
		}
		_input.Clear();
		_output.Clear();
		_manufactory = null;
		_workplace = null;
		_manufactoryDescriber = null;
		_enabled = false;
	}

	public void UpdateFragment()
	{
		if (_enabled && _manufactory.Enabled)
		{
			if (_manufactory.HasCurrentRecipe)
			{
				UpdateProductionRecipe(_manufactory.CurrentRecipe.ConsumesFuel);
			}
			else
			{
				bool showFuel = _manufactory.ProductionRecipes.Any((RecipeSpec recipe) => recipe.ConsumesFuel);
				UpdateProductionRecipe(showFuel);
			}
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private void UpdateProductionRecipe(bool showFuel)
	{
		string text = _loc.T(_productionProgressPhrase, _manufactory.ProductionProgress);
		_progressText.text = text;
		string param = $"{_manufactory.FuelRemaining * 100f:0}";
		_fuelRemaining.text = _loc.T(FuelRemainingLocKey, param);
		_fuelRemaining.ToggleDisplayStyle(showFuel);
		UpdateCraftingTime();
		_root.EnableInClassList(NoRecipeClass, !_manufactory.HasCurrentRecipe);
	}

	private void OnProductionRecipeChanged(object sender, EventArgs args)
	{
		Manufactory manufactory = _manufactory;
		ClearFragment();
		ShowFragment(manufactory);
		UpdateFragment();
	}

	private void AddProductionItem()
	{
		var (child, child2) = _manufactoryDescriber.DescribeRecipe(_manufactory.CurrentRecipe);
		_input.Add(child);
		_output.Add(child2);
	}

	private void OnWorkerChanged(object sender, WorkerChangedEventArgs workerChangedEventArgs)
	{
		UpdateCraftingTime();
	}

	private void UpdateCraftingTime()
	{
		bool flag = !_workplace || _workplace.DesiredWorkers > 0;
		if (_manufactory.HasCurrentRecipe & flag)
		{
			_craftingTime.text = _manufactoryDescriber.GetCraftingTime(_manufactory.CurrentRecipe, (!_workplace) ? 1 : _workplace.DesiredWorkers);
		}
		else
		{
			_craftingTime.text = "-";
		}
	}
}
