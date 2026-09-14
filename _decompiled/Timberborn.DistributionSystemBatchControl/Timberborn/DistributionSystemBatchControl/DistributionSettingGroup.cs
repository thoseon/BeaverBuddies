using System.Collections.Generic;
using Timberborn.BatchControl;
using UnityEngine.UIElements;

namespace Timberborn.DistributionSystemBatchControl;

internal class DistributionSettingGroup : IBatchControlRowItem, IUpdatableBatchControlRowItem, IClearableBatchControlRowItem
{
	private readonly List<GoodDistributionSettingItem> _goodDistributionSettingItems;

	public VisualElement Root { get; }

	public DistributionSettingGroup(VisualElement root, List<GoodDistributionSettingItem> goodDistributionSettingItems)
	{
		Root = root;
		_goodDistributionSettingItems = goodDistributionSettingItems;
	}

	public void UpdateRowItem()
	{
		for (int i = 0; i < _goodDistributionSettingItems.Count; i++)
		{
			_goodDistributionSettingItems[i].Update();
		}
	}

	public void ClearRowItem()
	{
		for (int i = 0; i < _goodDistributionSettingItems.Count; i++)
		{
			_goodDistributionSettingItems[i].Clear();
		}
	}
}
