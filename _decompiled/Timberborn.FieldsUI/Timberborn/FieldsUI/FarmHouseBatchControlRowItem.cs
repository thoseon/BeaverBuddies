using Timberborn.BatchControl;
using UnityEngine.UIElements;

namespace Timberborn.FieldsUI;

internal class FarmHouseBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private readonly FarmHouseToggle _farmHouseToggle;

	public VisualElement Root { get; }

	public FarmHouseBatchControlRowItem(VisualElement root, FarmHouseToggle farmHouseToggle)
	{
		Root = root;
		_farmHouseToggle = farmHouseToggle;
	}

	public void UpdateRowItem()
	{
		_farmHouseToggle.Update();
	}
}
