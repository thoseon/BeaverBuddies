using UnityEngine;

namespace Timberborn.PlatformUtilities;

public static class ApplicationPlatform
{
	public static bool IsMacOS()
	{
		RuntimePlatform platform = Application.platform;
		if ((uint)platform <= 1u)
		{
			return true;
		}
		return false;
	}

	public static bool IsWindows()
	{
		RuntimePlatform platform = Application.platform;
		if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor)
		{
			return true;
		}
		return false;
	}
}
