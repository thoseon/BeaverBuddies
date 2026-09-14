using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Timberborn.Modding;

public static class ModdedState
{
	public static bool HasOfficialMods { get; private set; }

	public static bool HasUnofficialMods { get; private set; }

	public static bool IsModded
	{
		get
		{
			if (!HasOfficialMods)
			{
				return HasUnofficialMods;
			}
			return true;
		}
	}

	public static void SetOfficialMods(IEnumerable<Mod> mods)
	{
		HasOfficialMods = true;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("official");
		foreach (Mod mod in mods)
		{
			stringBuilder.AppendLine("- " + mod.Manifest.Name + " (" + mod.Manifest.Version.Formatted + ")");
		}
		LogModded(stringBuilder.ToString());
	}

	public static void SetUnofficialMods()
	{
		if (!HasUnofficialMods)
		{
			HasUnofficialMods = true;
			LogModded("unofficial");
		}
	}

	private static void LogModded(string description)
	{
		Debug.Log("Modded: true, " + description);
	}
}
