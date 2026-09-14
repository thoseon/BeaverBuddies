using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Timberborn.ScreenSystem;

public static class ScreenResolutions
{
	private static readonly int MinResolutionHeight = 700;

	public static IEnumerable<ScreenResolution> AvailableResolutions()
	{
		if (Screen.resolutions.Length != 0)
		{
			return (from resolution in Screen.resolutions
				where resolution.height >= MinResolutionHeight
				orderby resolution.width, resolution.height
				select new ScreenResolution(resolution.width, resolution.height)).Distinct();
		}
		return FallbackResolutions();
	}

	private static IEnumerable<ScreenResolution> FallbackResolutions()
	{
		return new ScreenResolution[17]
		{
			new ScreenResolution(1024, 768),
			new ScreenResolution(1280, 720),
			new ScreenResolution(1280, 800),
			new ScreenResolution(1280, 1024),
			new ScreenResolution(1360, 768),
			new ScreenResolution(1366, 768),
			new ScreenResolution(1440, 900),
			new ScreenResolution(1536, 864),
			new ScreenResolution(1600, 900),
			new ScreenResolution(1680, 1050),
			new ScreenResolution(1920, 1080),
			new ScreenResolution(1920, 1200),
			new ScreenResolution(2048, 1152),
			new ScreenResolution(2560, 1080),
			new ScreenResolution(2560, 1440),
			new ScreenResolution(3440, 1440),
			new ScreenResolution(3840, 2160)
		};
	}
}
