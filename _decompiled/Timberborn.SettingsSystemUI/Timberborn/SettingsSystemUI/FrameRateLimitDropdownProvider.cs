using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.DropdownSystem;
using Timberborn.Localization;
using Timberborn.ScreenSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.SettingsSystemUI;

internal class FrameRateLimitDropdownProvider : IDropdownProvider, ILoadableSingleton
{
	private static readonly string FrameRateLimitValueLocKey = "Settings.Screen.FrameRateLimit.Value";

	private static readonly string FrameRateLimitUnlimitedLocKey = "Settings.Screen.FrameRateLimit.Unlimited";

	private readonly ScreenSettings _screenSettings;

	private readonly ILoc _loc;

	private ImmutableArray<string> _valuesFormatted;

	public IReadOnlyList<string> Items => _valuesFormatted;

	public FrameRateLimitDropdownProvider(ScreenSettings screenSettings, ILoc loc)
	{
		_screenSettings = screenSettings;
		_loc = loc;
	}

	public void Load()
	{
		_valuesFormatted = ScreenSettings.FrameRateLimitValues.Select(GetFormattedValue).ToImmutableArray();
	}

	public string GetValue()
	{
		int? value = ((_screenSettings.VSyncCount == 0) ? _screenSettings.FrameRateLimit : ((int?)null));
		return GetFormattedValue(value);
	}

	public void SetValue(string value)
	{
		_screenSettings.FrameRateLimit = ScreenSettings.FrameRateLimitValues[_valuesFormatted.IndexOf(value)];
	}

	private string GetFormattedValue(int? value)
	{
		if (!value.HasValue)
		{
			return _loc.T(FrameRateLimitUnlimitedLocKey);
		}
		return _loc.T(FrameRateLimitValueLocKey, value);
	}
}
