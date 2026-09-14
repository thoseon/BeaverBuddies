using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BatchControl;
using Timberborn.BlockSystem;
using Timberborn.CoreUI;
using Timberborn.InventorySystem;
using UnityEngine.UIElements;

namespace Timberborn.InventorySystemBatchControl;

internal class InventoryCapacityBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem, IFinishableBatchControlRowItem
{
	private readonly IReadOnlyList<InventoryCapacityBatchControlGood> _goods;

	private readonly Inventory _inventory;

	public VisualElement Root { get; }

	public InventoryCapacityBatchControlRowItem(VisualElement root, Inventory inventory, IEnumerable<InventoryCapacityBatchControlGood> goods)
	{
		Root = root;
		_inventory = inventory;
		_goods = goods.ToImmutableArray();
	}

	public void UpdateRowItem()
	{
		if (_inventory.Enabled && _inventory.GetComponent<BlockObject>().IsFinished)
		{
			for (int i = 0; i < _goods.Count; i++)
			{
				_goods[i].UpdateGoodAmount();
			}
			SetFinishedState(isFinished: true);
		}
		else
		{
			SetFinishedState(isFinished: false);
		}
	}

	public void SetFinishedState(bool isFinished)
	{
		Root.ToggleDisplayStyle(isFinished);
	}
}
