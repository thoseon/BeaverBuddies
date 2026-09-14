using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.DwellingSystem;
using UnityEngine.UIElements;

namespace Timberborn.DwellingSystemUI;

internal class DwellingBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem, IFinishableBatchControlRowItem
{
	private readonly Dwelling _dwelling;

	private readonly Label _info;

	public VisualElement Root { get; }

	public DwellingBatchControlRowItem(VisualElement root, Dwelling dwelling, Label info)
	{
		Root = root;
		_dwelling = dwelling;
		_info = info;
	}

	public void UpdateRowItem()
	{
		_info.text = $"{_dwelling.NumberOfDwellers} / {_dwelling.TotalSlots}";
	}

	public void SetFinishedState(bool isFinished)
	{
		Root.ToggleDisplayStyle(isFinished);
	}
}
