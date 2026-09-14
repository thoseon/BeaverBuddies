using Timberborn.BatchControl;
using Timberborn.DeteriorationSystem;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.DeteriorationSystemUI;

internal class DeteriorableBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private readonly ILoc _loc;

	private readonly Label _progressLabel;

	private readonly Deteriorable _deteriorable;

	private readonly Phrase _progressPhase = Phrase.New().FormatPercentFloored();

	public VisualElement Root { get; }

	public DeteriorableBatchControlRowItem(VisualElement root, ILoc loc, Label progressLabel, Deteriorable deteriorable)
	{
		Root = root;
		_loc = loc;
		_progressLabel = progressLabel;
		_deteriorable = deteriorable;
	}

	public void UpdateRowItem()
	{
		_progressLabel.text = _loc.T(_progressPhase, _deteriorable.DeteriorationProgress);
	}
}
