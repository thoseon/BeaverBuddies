using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.AutomationBuildings;
using Timberborn.Localization;
using Timberborn.SingletonSystem;

namespace Timberborn.AutomationBuildingsUI;

internal class MemoryModeDescriptions : ILoadableSingleton
{
	private static readonly string DescriptionLocKeyPrefix = "Building.Memory.Mode.";

	private static readonly string DescriptionLocKeyPostfix = ".Description";

	private static readonly string InputALocKey = "Automation.Input.A";

	private static readonly string InputBLocKey = "Automation.Input.B";

	private static readonly string ResetInputLocKey = "Automation.Input.Reset";

	private static readonly string ResetsWhenResetInputLocKey = "Automation.ResetsWhenResetInput";

	private readonly ILoc _loc;

	private readonly Dictionary<MemoryMode, string> _dictionary = new Dictionary<MemoryMode, string>();

	public MemoryModeDescriptions(ILoc loc)
	{
		_loc = loc;
	}

	public void Load()
	{
		string text = _loc.T(ResetsWhenResetInputLocKey, _loc.T(ResetInputLocKey));
		string param = _loc.T(InputALocKey);
		string param2 = _loc.T(InputBLocKey);
		foreach (MemoryMode item in Enum.GetValues(typeof(MemoryMode)).Cast<MemoryMode>())
		{
			string key = $"{DescriptionLocKeyPrefix}{item}{DescriptionLocKeyPostfix}";
			_dictionary.Add(item, _loc.T(key, param, param2) + "\n" + text);
		}
	}

	public string GetDescription(MemoryMode memoryMode)
	{
		return _dictionary[memoryMode];
	}
}
