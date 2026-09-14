using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;

namespace Timberborn.Workshops;

internal class RecipeGoodDisallower : BaseComponent, IAwakableComponent, IGoodDisallower, IFinishedStateListener
{
	private readonly Dictionary<string, int> _limits = new Dictionary<string, int>();

	public event EventHandler<DisallowedGoodsChangedEventArgs> DisallowedGoodsChanged;

	public void Awake()
	{
		DisableComponent();
	}

	public int AllowedAmount(string goodId)
	{
		if (!_limits.TryGetValue(goodId, out var value))
		{
			return 0;
		}
		return value;
	}

	public void UpdateAllowedAmounts(RecipeSpec recipeSpec)
	{
		ResetLimits();
		if (recipeSpec != null)
		{
			SetLimitsForRecipe(recipeSpec);
		}
		foreach (string key in _limits.Keys)
		{
			DisallowedGoodsChanged?.Invoke(this, new DisallowedGoodsChangedEventArgs(key));
		}
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	private void ResetLimits()
	{
		foreach (string item in _limits.Keys.ToList())
		{
			_limits[item] = 0;
		}
	}

	private void SetLimitsForRecipe(RecipeSpec recipeSpec)
	{
		foreach (GoodAmountSpec ingredient in recipeSpec.Ingredients)
		{
			_limits[ingredient.Id] = recipeSpec.GetCapacity(ingredient.ToGoodAmount());
		}
		foreach (GoodAmountSpec product in recipeSpec.Products)
		{
			_limits[product.Id] = recipeSpec.GetCapacity(product.ToGoodAmount());
		}
		if (recipeSpec.ConsumesFuel)
		{
			_limits[recipeSpec.Fuel] = recipeSpec.FuelCapacity;
		}
	}
}
