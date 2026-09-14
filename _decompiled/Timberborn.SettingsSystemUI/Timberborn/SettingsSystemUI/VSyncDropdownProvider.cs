using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.DropdownSystem;
using Timberborn.Localization;
using Timberborn.ScreenSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.SettingsSystemUI;

internal class VSyncDropdownProvider : IDropdownProvider, ILoadableSingleton
{
	private static readonly string VSync0LocKey = "Settings.Screen.VSync.0";

	private static readonly string VSync1LocKey = "Settings.Screen.VSync.1";

	private static readonly string VSync2LocKey = "Settings.Screen.VSync.2";

	private readonly ScreenSettings _screenSettings;

	private readonly ILoc _loc;

	private ImmutableArray<string> _valuesFormatted;

	public IReadOnlyList<string> Items => _valuesFormatted;

	public VSyncDropdownProvider(ScreenSettings screenSettings, ILoc loc)
	{
		_screenSettings = screenSettings;
		_loc = loc;
	}

	public void Load()
	{
		_valuesFormatted = ScreenSettings.VSyncValues.Select(GetFormattedValue).ToImmutableArray();
	}

	public string GetValue()
	{
		return GetFormattedValue(_screenSettings.VSyncCount);
	}

	public void SetValue(string value)
	{
		_screenSettings.VSyncCount = ScreenSettings.VSyncValues[_valuesFormatted.IndexOf(value)];
	}

	private string GetFormattedValue(int value)
	{
		return value switch
		{
			0 => _loc.T(VSync0LocKey), 
			1 => _loc.T(VSync1LocKey), 
			2 => _loc.T(VSync2LocKey), 
			_ => value.ToString(), 
		};
	}
}
