using Timberborn.Localization;

namespace Timberborn.FactionSystem;

public class FactionUnlockConditionDescriber
{
	private static readonly string WellbeingConditionLocKey = "FactionSelection.WellbeingCondition";

	private readonly FactionSpecService _factionSpecService;

	private readonly ILoc _loc;

	public FactionUnlockConditionDescriber(FactionSpecService factionSpecService, ILoc loc)
	{
		_factionSpecService = factionSpecService;
		_loc = loc;
	}

	public string Describe(FactionSpec factionSpec)
	{
		UnlockableFactionSpec spec = factionSpec.GetSpec<UnlockableFactionSpec>();
		if ((object)spec == null)
		{
			return "";
		}
		return DescribeUnlockCondition(spec);
	}

	private string DescribeUnlockCondition(UnlockableFactionSpec unlockableFactionSpec)
	{
		return _loc.T(WellbeingConditionLocKey, unlockableFactionSpec.AverageWellbeingToUnlock, GetPrerequisiteFactionDisplayName(unlockableFactionSpec));
	}

	private string GetPrerequisiteFactionDisplayName(UnlockableFactionSpec unlockableFactionSpec)
	{
		return _factionSpecService.GetFaction(unlockableFactionSpec.PrerequisiteFaction).DisplayName.Value;
	}
}
