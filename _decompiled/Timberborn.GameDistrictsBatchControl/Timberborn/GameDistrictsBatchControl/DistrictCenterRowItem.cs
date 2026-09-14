using Timberborn.BatchControl;
using Timberborn.GameDistricts;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsBatchControl;

internal class DistrictCenterRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private readonly DistrictCenter _districtCenter;

	private readonly Label _districtNameLabel;

	public VisualElement Root { get; }

	public DistrictCenterRowItem(VisualElement root, DistrictCenter districtCenter, Label districtNameLabel)
	{
		Root = root;
		_districtCenter = districtCenter;
		_districtNameLabel = districtNameLabel;
	}

	public void UpdateRowItem()
	{
		_districtNameLabel.text = _districtCenter.DistrictName;
	}
}
