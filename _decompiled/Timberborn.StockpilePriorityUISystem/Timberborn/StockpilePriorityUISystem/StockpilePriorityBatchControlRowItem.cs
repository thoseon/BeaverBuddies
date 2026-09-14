using Timberborn.BatchControl;
using UnityEngine.UIElements;

namespace Timberborn.StockpilePriorityUISystem;

public class StockpilePriorityBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private readonly StockpilePriorityToggle _stockpilePriorityToggle;

	public VisualElement Root { get; }

	public StockpilePriorityBatchControlRowItem(VisualElement root, StockpilePriorityToggle stockpilePriorityToggle)
	{
		Root = root;
		_stockpilePriorityToggle = stockpilePriorityToggle;
	}

	public void UpdateRowItem()
	{
		_stockpilePriorityToggle.Update();
	}
}
