using System.Collections.Generic;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.Goods;
using Timberborn.GoodsUI;
using Timberborn.InventorySystem;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.InventorySystemBatchControl;

public class InventoryCapacityBatchControlRowItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly GoodDescriber _goodDescriber;

	public InventoryCapacityBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, GoodDescriber goodDescriber)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_goodDescriber = goodDescriber;
	}

	public IBatchControlRowItem Create(Inventory inventory)
	{
		VisualElement visualElement = CreateRoot();
		VisualElement inventoryWrapper = visualElement.Q<VisualElement>("InventoryWrapper");
		IEnumerable<InventoryCapacityBatchControlGood> goods = CreateGoods(inventory, inventoryWrapper);
		return new InventoryCapacityBatchControlRowItem(visualElement, inventory, goods);
	}

	private VisualElement CreateRoot()
	{
		string elementName = "Game/BatchControl/InventoryCapacityBatchControlRowItem";
		return _visualElementLoader.LoadVisualElement(elementName);
	}

	private IEnumerable<InventoryCapacityBatchControlGood> CreateGoods(Inventory inventory, VisualElement inventoryWrapper)
	{
		foreach (StorableGoodAmount allowedGood in inventory.AllowedGoods)
		{
			string goodId = allowedGood.StorableGood.GoodId;
			DescribedGood describedGood = _goodDescriber.GetDescribedGood(goodId);
			VisualElement visualElement = CreateGoodElement();
			InitializeIcon(visualElement, describedGood);
			InitializeLabels(visualElement, inventory, goodId);
			InitializeTooltip(visualElement, describedGood);
			inventoryWrapper.Add(visualElement);
			yield return new InventoryCapacityBatchControlGood(visualElement.Q<Label>("CapacityAmount"), inventory, goodId);
		}
	}

	private VisualElement CreateGoodElement()
	{
		string elementName = "Game/BatchControl/InventoryCapacityBatchControlGood";
		return _visualElementLoader.LoadVisualElement(elementName);
	}

	private static void InitializeIcon(VisualElement goodElement, DescribedGood describedGood)
	{
		goodElement.Q<Image>("GoodIcon").sprite = describedGood.Icon;
	}

	private static void InitializeLabels(VisualElement goodElement, Inventory inventory, string goodId)
	{
		goodElement.Q<Label>("CapacityAmount").text = inventory.AmountInStock(goodId).ToString();
		goodElement.Q<Label>("CapacityLimit").text = inventory.LimitedAmount(goodId).ToString();
	}

	private void InitializeTooltip(VisualElement goodElement, DescribedGood describedGood)
	{
		_tooltipRegistrar.Register(goodElement, () => describedGood.DisplayName);
	}
}
