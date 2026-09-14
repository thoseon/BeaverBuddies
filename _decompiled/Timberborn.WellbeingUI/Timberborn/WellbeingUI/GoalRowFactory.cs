using Timberborn.CoreUI;
using Timberborn.FactionSystem;
using Timberborn.GameFactionSystem;
using Timberborn.Localization;
using Timberborn.Wellbeing;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

public class GoalRowFactory
{
	private static readonly string ProgressLocKey = "Goals.Progress";

	private static readonly string NotEligibleLocKey = "Goals.NotEligible";

	private static readonly string UnlockedLocKey = "Goals.Unlocked";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly FactionUnlockConditionDescriber _factionUnlockConditionDescriber;

	private readonly ILoc _loc;

	private readonly FactionUnlockingService _factionUnlockingService;

	private readonly FactionService _factionService;

	private readonly WellbeingService _wellbeingService;

	public GoalRowFactory(VisualElementLoader visualElementLoader, FactionUnlockConditionDescriber factionUnlockConditionDescriber, ILoc loc, FactionUnlockingService factionUnlockingService, FactionService factionService, WellbeingService wellbeingService)
	{
		_visualElementLoader = visualElementLoader;
		_factionUnlockConditionDescriber = factionUnlockConditionDescriber;
		_loc = loc;
		_factionUnlockingService = factionUnlockingService;
		_factionService = factionService;
		_wellbeingService = wellbeingService;
	}

	public VisualElement CreateRow(UnlockableFactionSpec unlockableFactionSpec)
	{
		FactionSpec spec = unlockableFactionSpec.GetSpec<FactionSpec>();
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/Population/GoalRow");
		StyleBackground backgroundImage = new StyleBackground(spec.Avatar.Asset);
		visualElement.Q<VisualElement>("Icon").style.backgroundImage = backgroundImage;
		visualElement.Q<Label>("Header").text = spec.DisplayName.Value;
		string text = _factionUnlockConditionDescriber.Describe(spec);
		visualElement.Q<Label>("Description").text = text;
		UpdateProgress(spec, unlockableFactionSpec, visualElement);
		return visualElement;
	}

	private void UpdateProgress(FactionSpec factionSpec, UnlockableFactionSpec unlockableFactionSpec, VisualElement goalRowElement)
	{
		if (_factionUnlockingService.IsLocked(factionSpec))
		{
			if (_factionService.Current.Id == unlockableFactionSpec.PrerequisiteFaction)
			{
				string progress = $"{_wellbeingService.AverageGlobalWellbeing} " + $"/ {unlockableFactionSpec.AverageWellbeingToUnlock}";
				UpdateProgress(progress, goalRowElement);
			}
			else
			{
				UpdateProgress(_loc.T(NotEligibleLocKey), goalRowElement);
			}
		}
		else
		{
			UpdateProgress(_loc.T(UnlockedLocKey), goalRowElement);
		}
	}

	private void UpdateProgress(string progress, VisualElement goalRowElement)
	{
		goalRowElement.Q<Label>("Progress").text = _loc.T(ProgressLocKey) + " " + progress;
	}
}
