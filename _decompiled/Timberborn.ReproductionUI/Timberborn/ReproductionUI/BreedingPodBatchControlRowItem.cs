using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.Reproduction;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.ReproductionUI;

internal class BreedingPodBatchControlRowItem : IUpdatableBatchControlRowItem, IBatchControlRowItem, IFinishableBatchControlRowItem
{
	private readonly ILoc _loc;

	private readonly BreedingPod _breedingPod;

	private readonly Label _progressLabel;

	private readonly Phrase _progressPhrase = Phrase.New().FormatPercentFloored();

	public VisualElement Root { get; }

	public BreedingPodBatchControlRowItem(VisualElement root, ILoc loc, BreedingPod breedingPod, Label progressLabel)
	{
		Root = root;
		_loc = loc;
		_breedingPod = breedingPod;
		_progressLabel = progressLabel;
	}

	public void UpdateRowItem()
	{
		_progressLabel.text = _loc.T(_progressPhrase, _breedingPod.CalculateProgress());
	}

	public void SetFinishedState(bool isFinished)
	{
		Root.ToggleDisplayStyle(isFinished);
	}
}
