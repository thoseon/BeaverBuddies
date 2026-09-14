using UnityEngine;

namespace Timberborn.Modding;

public static class ModPlayerPrefsHelper
{
	private static readonly string ModEnabledFormat = "ModEnabled.{0}";

	private static readonly string ModPriorityFormat = "ModPriority.{0}";

	public static bool IsModEnabled(Mod mod)
	{
		string modEnabledKey = GetModEnabledKey(mod);
		if (PlayerPrefs.HasKey(modEnabledKey))
		{
			return PlayerPrefs.GetInt(modEnabledKey) == 1;
		}
		return true;
	}

	public static bool IsModEnabled(ModDirectory modDirectory, ModManifest modManifest)
	{
		string modEnabledKey = GetModEnabledKey(modDirectory, modManifest);
		if (PlayerPrefs.HasKey(modEnabledKey))
		{
			return PlayerPrefs.GetInt(modEnabledKey) == 1;
		}
		return true;
	}

	public static void ToggleMod(bool enabled, Mod mod)
	{
		PlayerPrefs.SetInt(GetModEnabledKey(mod), enabled ? 1 : 0);
	}

	public static int GetModPriority(Mod mod)
	{
		string modPriorityKey = GetModPriorityKey(mod);
		if (!PlayerPrefs.HasKey(modPriorityKey))
		{
			return 0;
		}
		return PlayerPrefs.GetInt(modPriorityKey);
	}

	public static void IncreaseModPriority(Mod mod)
	{
		SetModPriority(mod, GetModPriority(mod) + 1);
	}

	public static void DecreaseModPriority(Mod mod)
	{
		SetModPriority(mod, GetModPriority(mod) - 1);
	}

	public static void SetModPriority(Mod mod, int priority)
	{
		PlayerPrefs.SetInt(GetModPriorityKey(mod), priority);
	}

	public static void ResetModPriority(Mod mod)
	{
		PlayerPrefs.DeleteKey(GetModPriorityKey(mod));
	}

	private static string GetModEnabledKey(Mod mod)
	{
		return string.Format(ModEnabledFormat, GetModKey(mod.ModDirectory, mod.Manifest));
	}

	private static string GetModEnabledKey(ModDirectory modDirectory, ModManifest modManifest)
	{
		return string.Format(ModEnabledFormat, GetModKey(modDirectory, modManifest));
	}

	private static string GetModPriorityKey(Mod mod)
	{
		return string.Format(ModPriorityFormat, GetModKey(mod.ModDirectory, mod.Manifest));
	}

	private static string GetModKey(ModDirectory modDirectory, ModManifest modManifest)
	{
		return modDirectory.DisplaySource + "." + modDirectory.OriginName + "." + modManifest.Id;
	}
}
