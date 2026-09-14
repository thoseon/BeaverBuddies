using Timberborn.BatchControl;
using Timberborn.Beavers;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.BeaversUI;

internal class AdulthoodBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private readonly ILoc _loc;

	private readonly Label _progressLabel;

	private readonly Child _child;

	private readonly Phrase _progressPhrase = Phrase.New().FormatPercentFloored();

	public VisualElement Root { get; }

	public AdulthoodBatchControlRowItem(VisualElement root, ILoc loc, Label progressLabel, Child child)
	{
		Root = root;
		_loc = loc;
		_progressLabel = progressLabel;
		_child = child;
	}

	public void UpdateRowItem()
	{
		_progressLabel.text = _loc.T(_progressPhrase, _child.GrowthProgress);
	}
}
