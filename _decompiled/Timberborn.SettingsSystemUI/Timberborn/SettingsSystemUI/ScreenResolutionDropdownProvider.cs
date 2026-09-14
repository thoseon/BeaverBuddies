using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.ScreenSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.SettingsSystemUI;

internal class ScreenResolutionDropdownProvider : IDropdownProvider, ILoadableSingleton
{
	private readonly ScreenSettings _screenSettings;

	private ImmutableArray<ScreenResolution> _resolutions;

	private ImmutableArray<string> _resolutionsFormatted;

	public IReadOnlyList<string> Items => _resolutionsFormatted;

	public ScreenResolutionDropdownProvider(ScreenSettings screenSettings)
	{
		_screenSettings = screenSettings;
	}

	public void Load()
	{
		_resolutions = ScreenResolutions.AvailableResolutions().Reverse().ToImmutableArray();
		_resolutionsFormatted = _resolutions.Select(GetFormattedResolution).ToImmutableArray();
	}

	public string GetValue()
	{
		return GetFormattedResolution(_screenSettings.ScreenResolution);
	}

	public void SetValue(string value)
	{
		_screenSettings.ScreenResolution = _resolutions[_resolutionsFormatted.IndexOf(value)];
	}

	private static string GetFormattedResolution(ScreenResolution screenResolution)
	{
		return $"{screenResolution.Width} {SpecialStrings.SizeSeparator} {screenResolution.Height}";
	}
}
