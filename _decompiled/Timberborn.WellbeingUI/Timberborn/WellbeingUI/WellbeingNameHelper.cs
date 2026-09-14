using Timberborn.Bots;
using Timberborn.Localization;
using Timberborn.Wellbeing;

namespace Timberborn.WellbeingUI;

public class WellbeingNameHelper
{
	private static readonly string WellbeingLocKey = "Wellbeing.DisplayName";

	private static readonly string ConditionLocKey = "Condition.DisplayName";

	private readonly ILoc _loc;

	public WellbeingNameHelper(ILoc loc)
	{
		_loc = loc;
	}

	public string GetWellbeingName(WellbeingTracker wellbeingTracker)
	{
		return _loc.T(wellbeingTracker.HasComponent<BotSpec>() ? ConditionLocKey : WellbeingLocKey);
	}
}
