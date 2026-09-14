using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.DeteriorationSystem;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.DeteriorationSystemUI;

public class DeteriorableBatchControlRowItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly ILoc _loc;

	private readonly Phrase _durabilityPhrase = Phrase.New("Bot.Durability").FormatPercentFloored();

	public DeteriorableBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_loc = loc;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		Deteriorable deteriorable = entity.GetComponent<Deteriorable>();
		if (deteriorable != null)
		{
			string elementName = "Game/BatchControl/DeteriorableBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			Label progressLabel = visualElement.Q<Label>("Progress");
			_tooltipRegistrar.Register(visualElement, () => _loc.T(_durabilityPhrase, deteriorable.DeteriorationProgress));
			return new DeteriorableBatchControlRowItem(visualElement, _loc, progressLabel, deteriorable);
		}
		return null;
	}
}
