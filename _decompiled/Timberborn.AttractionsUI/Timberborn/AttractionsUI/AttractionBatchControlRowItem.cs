using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.EnterableSystem;
using UnityEngine.UIElements;

namespace Timberborn.AttractionsUI;

internal class AttractionBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem, IFinishableBatchControlRowItem
{
	private readonly Label _capacityLabel;

	private readonly Enterable _enterable;

	public VisualElement Root { get; }

	public AttractionBatchControlRowItem(VisualElement root, Label capacityLabel, Enterable enterable)
	{
		Root = root;
		_capacityLabel = capacityLabel;
		_enterable = enterable;
	}

	public void UpdateRowItem()
	{
		_capacityLabel.text = $"{_enterable.NumberOfEnterersInside} / {_enterable.EnterableSpec.CapacityFinished}";
	}

	public void SetFinishedState(bool isFinished)
	{
		Root.ToggleDisplayStyle(isFinished);
	}
}
