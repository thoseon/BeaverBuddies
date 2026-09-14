using System.Collections.Generic;
using System.Linq;
using Timberborn.Modding;
using Timberborn.Versioning;

namespace Timberborn.ModdingUI;

internal class ModWarningUpdater
{
	public void Update(Dictionary<Mod, ModItem> modItems)
	{
		foreach (ModItem value in modItems.Values)
		{
			if (!ModPlayerPrefsHelper.IsModEnabled(value.Mod) || (ValidateRequiredMods(value, modItems) && ValidateMinimumGameVersion(value)))
			{
				value.SetWarning(ModWarningReason.None, string.Empty);
			}
		}
	}

	private static bool ValidateRequiredMods(ModItem modItem, Dictionary<Mod, ModItem> modItems)
	{
		foreach (VersionedMod requiredMod in modItem.ModManifest.RequiredMods)
		{
			if (IsRequiredModNotInstalled(modItems, requiredMod))
			{
				modItem.SetWarning(ModWarningReason.MissingRequiredMod, requiredMod.Id);
				return false;
			}
			if (!ValidateRequiredMod(modItem, modItems, requiredMod))
			{
				return false;
			}
		}
		return true;
	}

	private static bool ValidateRequiredMod(ModItem modItem, Dictionary<Mod, ModItem> modItems, VersionedMod requiredModDefinition)
	{
		foreach (Mod item in modItems.Keys.Where((Mod mod) => mod.Manifest.Id == requiredModDefinition.Id))
		{
			if (IsRequiredModDisabled(item))
			{
				modItem.SetWarning(ModWarningReason.RequiredModNotEnabled, item.DisplayName);
				continue;
			}
			if (IsRequiredModBelowMinimumVersion(item, requiredModDefinition))
			{
				modItem.SetWarning(ModWarningReason.RequiredModInvalidVersion, item.DisplayName);
				continue;
			}
			if (IsRequiredModBelowInLoadOrder(modItem, modItems[item]))
			{
				modItem.SetWarning(ModWarningReason.RequiredModInvalidOrder, item.DisplayName);
				continue;
			}
			return true;
		}
		return false;
	}

	private static bool IsRequiredModNotInstalled(Dictionary<Mod, ModItem> modItems, VersionedMod requiredMod)
	{
		return modItems.Keys.All((Mod mod) => mod.Manifest.Id != requiredMod.Id);
	}

	private static bool IsRequiredModDisabled(Mod requiredModInstance)
	{
		return !ModPlayerPrefsHelper.IsModEnabled(requiredModInstance);
	}

	private static bool IsRequiredModBelowMinimumVersion(Mod requiredMod, VersionedMod requiredModDefinition)
	{
		return !requiredMod.Manifest.Version.IsEqualOrHigherThan(requiredModDefinition.MinimumVersion);
	}

	private static bool IsRequiredModBelowInLoadOrder(ModItem modItem, ModItem requiredMod)
	{
		return GetModLoadOrder(requiredMod) > GetModLoadOrder(modItem);
	}

	private static int GetModLoadOrder(ModItem modItem)
	{
		return modItem.Root.parent.IndexOf(modItem.Root);
	}

	private static bool ValidateMinimumGameVersion(ModItem modItem)
	{
		if (GameVersions.CurrentVersion.IsEqualOrHigherThan(modItem.ModManifest.MinimumGameVersion) || GameVersions.CurrentVersion.IsDevelopmentVersion)
		{
			return true;
		}
		modItem.SetWarning(ModWarningReason.InvalidGameVersion, modItem.ModManifest.MinimumGameVersion.Formatted);
		return false;
	}
}
