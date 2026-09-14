using Timberborn.BatchControl;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

internal class WellbeingBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private readonly WellbeingSummary _wellbeingSummary;

	public VisualElement Root { get; }

	public WellbeingBatchControlRowItem(VisualElement root, WellbeingSummary wellbeingSummary)
	{
		Root = root;
		_wellbeingSummary = wellbeingSummary;
	}

	public void UpdateRowItem()
	{
		_wellbeingSummary.UpdateContent();
	}
}
