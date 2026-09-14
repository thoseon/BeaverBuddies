using System.Text;
using Timberborn.BonusSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;
using Timberborn.UIFormatters;
using Timberborn.Wellbeing;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

public class WellbeingBonusTooltipFactory
{
	private static readonly string NextTierLocKey = "Wellbeing.NextTier";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly IWellbeingTierService _wellbeingTierService;

	private readonly BonusTypeSpecService _bonusTypeSpecService;

	private readonly WellbeingNameHelper _wellbeingNameHelper;

	private readonly WellbeingLimitService _wellbeingLimitService;

	private readonly BonusDescriber _bonusDescriber;

	private readonly StringBuilder _contentBuilder = new StringBuilder();

	private readonly Phrase _bonusValuePhrase = Phrase.New().FormatPercentRounded();

	public WellbeingBonusTooltipFactory(VisualElementLoader visualElementLoader, ILoc loc, IWellbeingTierService wellbeingTierService, BonusTypeSpecService bonusTypeSpecService, WellbeingNameHelper wellbeingNameHelper, WellbeingLimitService wellbeingLimitService, BonusDescriber bonusDescriber)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_wellbeingTierService = wellbeingTierService;
		_bonusTypeSpecService = bonusTypeSpecService;
		_wellbeingNameHelper = wellbeingNameHelper;
		_wellbeingLimitService = wellbeingLimitService;
		_bonusDescriber = bonusDescriber;
	}

	public VisualElement Create(WellbeingTracker wellbeingTracker, string bonusId)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/WellbeingBonusTooltip");
		BonusTypeSpec spec = _bonusTypeSpecService.GetSpec(bonusId);
		int wellbeing = wellbeingTracker.Wellbeing;
		UpdateWellbeingBonus(wellbeingTracker, spec, wellbeing);
		UpdateNextTierWellbeingBonus(visualElement, wellbeingTracker, spec, wellbeing);
		UpdateNeedPenalties(wellbeingTracker, spec);
		visualElement.Q<Label>("Description").text = _contentBuilder.ToStringWithoutNewLineEnd();
		visualElement.Q<Label>("Name").text = spec.DisplayName.Value;
		_contentBuilder.Clear();
		return visualElement;
	}

	private void UpdateWellbeingBonus(WellbeingTracker wellbeingTracker, BonusTypeSpec bonusTypeSpec, int wellbeing)
	{
		_wellbeingTierService.TryGetTierBonus(wellbeingTracker, bonusTypeSpec.Id, wellbeing, out var tierBonus);
		string arg = FormatBonus(tierBonus.Bonus);
		string wellbeingName = _wellbeingNameHelper.GetWellbeingName(wellbeingTracker);
		string text = $"{wellbeingName} {tierBonus.Wellbeing}: {arg}";
		if (tierBonus.Bonus > 0f)
		{
			text = _bonusDescriber.ColorPositive(text);
		}
		_contentBuilder.AppendLine(text);
	}

	private void UpdateNextTierWellbeingBonus(VisualElement root, WellbeingTracker wellbeingTracker, BonusTypeSpec bonusTypeSpec, int wellbeing)
	{
		bool num = _wellbeingTierService.TryGetNextTierBonus(wellbeingTracker, bonusTypeSpec.Id, wellbeing, out var nextTierBonus);
		int maxWellbeing = _wellbeingLimitService.GetMaxWellbeing(wellbeingTracker);
		VisualElement visualElement = root.Q<VisualElement>("NextTier");
		if (num && nextTierBonus.Wellbeing <= maxWellbeing)
		{
			visualElement.ToggleDisplayStyle(visible: true);
			string text = FormatBonus(nextTierBonus.Bonus);
			string text2 = _loc.T(NextTierLocKey);
			string wellbeingName = _wellbeingNameHelper.GetWellbeingName(wellbeingTracker);
			root.Q<Label>("NextTierDescription").text = $"{text2}:\n{wellbeingName} {nextTierBonus.Wellbeing}: {text}";
		}
		else
		{
			visualElement.ToggleDisplayStyle(visible: false);
		}
	}

	private void UpdateNeedPenalties(WellbeingTracker wellbeingTracker, BonusTypeSpec bonusTypeSpec)
	{
		NeedManager component = wellbeingTracker.GetComponent<NeedManager>();
		foreach (NeedSpec needSpec in component.NeedSpecs)
		{
			PunitiveNeedSpec spec = needSpec.GetSpec<PunitiveNeedSpec>();
			if ((object)spec == null)
			{
				continue;
			}
			foreach (BonusSpec penalty in spec.Penalties)
			{
				if (penalty.Id == bonusTypeSpec.Id && !component.NeedIsFavorable(needSpec.Id))
				{
					AddNeedPenalty(penalty, needSpec.DisplayName.Value);
				}
			}
		}
	}

	private void AddNeedPenalty(BonusSpec penalty, string needDisplayName)
	{
		string text = FormatBonus(penalty.MultiplierDelta);
		string description = needDisplayName + ": " + text;
		_contentBuilder.AppendLine(_bonusDescriber.ColorNegative(description));
	}

	private string FormatBonus(float bonusValue)
	{
		string text = _loc.T(_bonusValuePhrase, bonusValue);
		if (bonusValue > 0f)
		{
			return "+" + text;
		}
		return text;
	}
}
