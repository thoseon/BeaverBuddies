using System.Collections.Generic;
using System.Linq;
using Timberborn.CoreUI;
using Timberborn.Goods;
using Timberborn.GoodsUI;
using Timberborn.InventorySystem;
using Timberborn.Workshops;
using UnityEngine.UIElements;

namespace Timberborn.InventorySystemUI;

public class InventoryRowUpdater
{
	private readonly GoodDescriber _goodDescriber;

	private readonly InformationalRowsFactory _informationalRowsFactory;

	public InventoryRowUpdater(GoodDescriber goodDescriber, InformationalRowsFactory informationalRowsFactory)
	{
		_goodDescriber = goodDescriber;
		_informationalRowsFactory = informationalRowsFactory;
	}

	public void AddRows(ScrollView inventoryContent, Inventory inventory, List<InformationalRow> rows, RecipeSpec recipeSpec)
	{
		List<StorableGoodAmount> goods = inventory.AllowedGoods.OrderBy((StorableGoodAmount good) => _goodDescriber.Describe(good.StorableGood.GoodId)).ToList();
		if (recipeSpec != null)
		{
			AddRecipeRows(inventoryContent, inventory, rows, goods, recipeSpec);
		}
		AddRemainingRows(inventoryContent, inventory, rows, goods);
	}

	public void UpdateRowsVisibility(VisualElement root, VisualElement isEmpty, Inventory inventory, List<InformationalRow> rows)
	{
		if ((bool)inventory && inventory.Enabled)
		{
			bool flag = false;
			root.ToggleDisplayStyle(visible: true);
			foreach (InformationalRow row in rows)
			{
				if (ShouldShow(inventory, row.GoodId))
				{
					row.ShowUpdated();
					flag = true;
				}
				else
				{
					row.Hide();
				}
			}
			isEmpty.ToggleDisplayStyle(!flag);
		}
		else
		{
			root.ToggleDisplayStyle(visible: false);
		}
	}

	private void AddRecipeRows(ScrollView inventoryContent, Inventory inventory, List<InformationalRow> rows, List<StorableGoodAmount> goods, RecipeSpec currentRecipe)
	{
		foreach (StorableGoodAmount item3 in goods.Where((StorableGoodAmount good) => good.StorableGood.IsOnlyGivable))
		{
			StorableGood storableGood = item3.StorableGood;
			if (IsInputOrFuel(currentRecipe, storableGood.GoodId))
			{
				InformationalRow item = _informationalRowsFactory.CreateInputRowWithLimit(storableGood, inventory, inventoryContent);
				rows.Add(item);
			}
		}
		foreach (StorableGoodAmount item4 in goods.Where((StorableGoodAmount good) => good.StorableGood.IsOnlyTakeable))
		{
			StorableGood storableGood2 = item4.StorableGood;
			if (IsOutput(currentRecipe, storableGood2.GoodId))
			{
				InformationalRow item2 = _informationalRowsFactory.CreateOutputRowWithLimit(storableGood2, inventory, inventoryContent);
				rows.Add(item2);
			}
		}
	}

	private void AddRemainingRows(ScrollView inventoryContent, Inventory inventory, List<InformationalRow> rows, List<StorableGoodAmount> goods)
	{
		foreach (StorableGoodAmount good in goods)
		{
			StorableGood storableGood = good.StorableGood;
			if (rows.All((InformationalRow row) => row.GoodId != storableGood.GoodId))
			{
				InformationalRow item = _informationalRowsFactory.CreateSimpleRowWithoutLimit(storableGood, inventory, inventoryContent);
				rows.Add(item);
			}
		}
	}

	private static bool IsInputOrFuel(RecipeSpec currentRecipe, string goodId)
	{
		if (!currentRecipe.Ingredients.Any((GoodAmountSpec goodAmount) => goodAmount.Id == goodId))
		{
			return currentRecipe.Fuel == goodId;
		}
		return true;
	}

	private static bool IsOutput(RecipeSpec currentRecipe, string goodId)
	{
		return currentRecipe.Products.Any((GoodAmountSpec goodAmount) => goodAmount.Id == goodId);
	}

	private static bool ShouldShow(Inventory inventory, string goodId)
	{
		bool num = inventory.LimitedAmount(goodId) > 0;
		bool flag = inventory.AmountInStock(goodId) > 0;
		return num | flag;
	}
}
