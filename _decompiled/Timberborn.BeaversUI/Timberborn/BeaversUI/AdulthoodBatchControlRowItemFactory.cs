using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.Beavers;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.BeaversUI;

public class AdulthoodBatchControlRowItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly ILoc _loc;

	private readonly Phrase _progressPhrase = Phrase.New("Beaver.Adulthood").FormatPercentFloored();

	public AdulthoodBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_loc = loc;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		Child child = entity.GetComponent<Child>();
		if (child != null && ((BaseComponent)(object)child).Enabled)
		{
			string elementName = "Game/BatchControl/AdulthoodBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			Label progressLabel = visualElement.Q<Label>("Progress");
			_tooltipRegistrar.Register(visualElement, () => _loc.T(_progressPhrase, child.GrowthProgress));
			return new AdulthoodBatchControlRowItem(visualElement, _loc, progressLabel, child);
		}
		return null;
	}
}
