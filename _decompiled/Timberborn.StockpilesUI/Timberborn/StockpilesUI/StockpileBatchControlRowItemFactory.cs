using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.Goods;
using Timberborn.GoodsUI;
using Timberborn.InventorySystem;
using Timberborn.Localization;
using Timberborn.Stockpiles;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.StockpilesUI;

public class StockpileBatchControlRowItemFactory
{
	private static readonly string InStockLocKey = "Inventory.InStock";

	private static readonly string EmptyLocKey = "Inventory.IsEmpty";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly GoodDescriber _goodDescriber;

	private readonly ILoc _loc;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly StringBuilder _tooltipText = new StringBuilder();

	private readonly List<string> _orderedItems = new List<string>();

	public StockpileBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, GoodDescriber goodDescriber, ILoc loc, DropdownItemsSetter dropdownItemsSetter)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_goodDescriber = goodDescriber;
		_loc = loc;
		_dropdownItemsSetter = dropdownItemsSetter;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		Stockpile component = entity.GetComponent<Stockpile>();
		if (component != null)
		{
			string elementName = "Game/BatchControl/StockpileBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			Inventory inventory = component.Inventory;
			SingleGoodAllower component2 = component.GetComponent<SingleGoodAllower>();
			visualElement.Q<Label>("CapacityLimit").text = inventory.Capacity.ToString();
			_tooltipRegistrar.RegisterUpdatable(visualElement.Q<VisualElement>("CapacityWrapper"), () => GetTooltipText(inventory));
			StockpileDropdownProvider component3 = component.GetComponent<StockpileDropdownProvider>();
			Dropdown dropdown = visualElement.Q<Dropdown>("GoodSelectionButton");
			_dropdownItemsSetter.SetItems(dropdown, component3);
			return new StockpileBatchControlRowItem(visualElement, inventory, component2, visualElement.Q<Label>("CapacityAmount"), dropdown, visualElement.Q<VisualElement>("Fill"));
		}
		return null;
	}

	private string GetTooltipText(Inventory inventory)
	{
		_tooltipText.Clear();
		_tooltipText.AppendLine("<b>" + _loc.T(InStockLocKey) + "</b>");
		IOrderedEnumerable<string> collection = from good in inventory.Stock
			select _goodDescriber.Describe(good) into info
			orderby info
			select info;
		_orderedItems.AddRange(collection);
		if (_orderedItems.Count > 0)
		{
			foreach (string orderedItem in _orderedItems)
			{
				_tooltipText.AppendLine(orderedItem);
			}
			_orderedItems.Clear();
			return _tooltipText.ToStringWithoutNewLineEnd();
		}
		return _loc.T(EmptyLocKey);
	}
}
