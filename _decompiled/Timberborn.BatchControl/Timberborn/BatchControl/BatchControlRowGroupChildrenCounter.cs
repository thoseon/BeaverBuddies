using UnityEngine.UIElements;

namespace Timberborn.BatchControl;

internal class BatchControlRowGroupChildrenCounter : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private readonly Label _counter;

	private BatchControlRowGroup _rowGroup;

	public VisualElement Root { get; }

	public BatchControlRowGroupChildrenCounter(VisualElement root, Label counter)
	{
		Root = root;
		_counter = counter;
	}

	public void SetRowGroup(BatchControlRowGroup rowGroup)
	{
		_rowGroup = rowGroup;
	}

	public void UpdateRowItem()
	{
		_counter.text = " (" + _rowGroup.VisibleChildrenCount + ")";
	}
}
