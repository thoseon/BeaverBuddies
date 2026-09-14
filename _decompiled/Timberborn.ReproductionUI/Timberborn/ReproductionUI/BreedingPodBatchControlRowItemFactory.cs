using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.Reproduction;
using Timberborn.TooltipSystem;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.ReproductionUI;

public class BreedingPodBatchControlRowItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly ILoc _loc;

	private readonly Phrase _progressPhrase = Phrase.New("Breeding.Progress").FormatPercentFloored();

	public BreedingPodBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_loc = loc;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		BreedingPod breedingPod = entity.GetComponent<BreedingPod>();
		if ((bool)breedingPod)
		{
			string elementName = "Game/BatchControl/BreedingPodBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			_tooltipRegistrar.Register(visualElement, () => _loc.T(_progressPhrase, breedingPod.CalculateProgress()));
			return new BreedingPodBatchControlRowItem(visualElement, _loc, breedingPod, visualElement.Q<Label>("Progress"));
		}
		return null;
	}
}
