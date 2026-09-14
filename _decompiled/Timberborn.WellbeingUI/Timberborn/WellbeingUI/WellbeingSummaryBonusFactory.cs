using Timberborn.BonusSystem;
using Timberborn.CoreUI;
using Timberborn.TooltipSystem;
using Timberborn.Wellbeing;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

public class WellbeingSummaryBonusFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly WellbeingBonusTooltipFactory _wellbeingBonusTooltipFactory;

	private readonly BonusTypeSpecService _bonusTypeSpecService;

	public WellbeingSummaryBonusFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, WellbeingBonusTooltipFactory wellbeingBonusTooltipFactory, BonusTypeSpecService bonusTypeSpecService)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_wellbeingBonusTooltipFactory = wellbeingBonusTooltipFactory;
		_bonusTypeSpecService = bonusTypeSpecService;
	}

	public WellbeingSummaryBonus Create(BonusManager bonusManager, string bonusId)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/WellbeingSummaryBonusFragment");
		Label bonusValue = visualElement.Q<Label>("Value");
		visualElement.Q<Image>("Icon").sprite = _bonusTypeSpecService.GetSpec(bonusId).Icon.Asset;
		WellbeingSummaryBonus wellbeingSummaryBonus = new WellbeingSummaryBonus(visualElement, bonusManager, bonusValue, bonusId);
		wellbeingSummaryBonus.UpdateBonus();
		WellbeingTracker wellbeingTracker = bonusManager.GetComponent<WellbeingTracker>();
		_tooltipRegistrar.Register(visualElement, () => _wellbeingBonusTooltipFactory.Create(wellbeingTracker, bonusId));
		return wellbeingSummaryBonus;
	}
}
