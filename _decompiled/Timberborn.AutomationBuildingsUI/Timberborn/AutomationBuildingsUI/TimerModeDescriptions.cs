using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.AutomationBuildings;
using Timberborn.Localization;
using Timberborn.SingletonSystem;

namespace Timberborn.AutomationBuildingsUI;

internal class TimerModeDescriptions : ILoadableSingleton
{
	private static readonly string DescriptionLocKeyPrefix = "Building.Timer.Mode.";

	private static readonly string DescriptionLocKeyPostfix = ".Description";

	private static readonly string InputALocKey = "Automation.Input.A";

	private static readonly string ResetInputLocKey = "Automation.Input.Reset";

	private static readonly string ResetsWhenResetInputLocKey = "Automation.ResetsWhenResetInput";

	private static readonly string TimeALocKey = "Building.Timer.TimeA";

	private static readonly string TimeBLocKey = "Building.Timer.TimeB";

	private readonly Dictionary<TimerMode, string> _dictionary = new Dictionary<TimerMode, string>();

	private readonly ILoc _loc;

	public TimerModeDescriptions(ILoc loc)
	{
		_loc = loc;
	}

	public void Load()
	{
		string text = _loc.T(ResetsWhenResetInputLocKey, _loc.T(ResetInputLocKey));
		string param = _loc.T(InputALocKey);
		string param2 = _loc.T(TimeALocKey);
		string param3 = _loc.T(TimeBLocKey);
		foreach (TimerMode item in Enum.GetValues(typeof(TimerMode)).Cast<TimerMode>())
		{
			string key = $"{DescriptionLocKeyPrefix}{item}{DescriptionLocKeyPostfix}";
			_dictionary.Add(item, _loc.T(key, param, param2, param3) + "\n" + text);
		}
	}

	public string GetDescription(TimerMode timerMode)
	{
		return _dictionary[timerMode];
	}
}
