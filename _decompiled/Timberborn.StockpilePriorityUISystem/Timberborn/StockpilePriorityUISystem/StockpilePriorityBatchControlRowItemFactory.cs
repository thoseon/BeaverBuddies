using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.StockpilePrioritySystem;
using UnityEngine.UIElements;

namespace Timberborn.StockpilePriorityUISystem;

public class StockpilePriorityBatchControlRowItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly StockpilePriorityToggleFactory _stockpilePriorityToggleFactory;

	public StockpilePriorityBatchControlRowItemFactory(VisualElementLoader visualElementLoader, StockpilePriorityToggleFactory stockpilePriorityToggleFactory)
	{
		_visualElementLoader = visualElementLoader;
		_stockpilePriorityToggleFactory = stockpilePriorityToggleFactory;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		StockpilePriority component = entity.GetComponent<StockpilePriority>();
		if (component != null)
		{
			string elementName = "Game/BatchControl/SelectionToggleBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			StockpilePriorityToggle stockpilePriorityToggle = _stockpilePriorityToggleFactory.Create(visualElement);
			stockpilePriorityToggle.Show(component);
			return new StockpilePriorityBatchControlRowItem(visualElement, stockpilePriorityToggle);
		}
		return null;
	}
}
