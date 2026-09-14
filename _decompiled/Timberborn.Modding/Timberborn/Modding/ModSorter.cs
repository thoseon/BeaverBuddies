using System.Collections.Generic;
using System.Linq;

namespace Timberborn.Modding;

public class ModSorter
{
	public IEnumerable<Mod> Sort(IEnumerable<Mod> mods)
	{
		return (from x in SortByDependencies(mods.OrderBy((Mod mod) => mod.DisplayName)).Select((Mod x, int i) => new
			{
				Value = x,
				OriginalIndex = i
			})
			orderby x.OriginalIndex - ModPlayerPrefsHelper.GetModPriority(x.Value)
			select x).Select((x, int i) =>
		{
			int modPriority = ModPlayerPrefsHelper.GetModPriority(x.Value);
			if (modPriority != 0 && i != x.OriginalIndex - modPriority)
			{
				ModPlayerPrefsHelper.SetModPriority(x.Value, x.OriginalIndex - i);
			}
			return x.Value;
		});
	}

	private static IEnumerable<Mod> SortByDependencies(IEnumerable<Mod> mods)
	{
		Dictionary<Mod, List<VersionedMod>> modsDependencies = mods.ToDictionary((Mod mod) => mod, (Mod mod) => mod.Manifest.RequiredMods.Concat(mod.Manifest.OptionalMods).ToList());
		while (modsDependencies.Count > 0)
		{
			int minDependenciesCount = modsDependencies.Min((KeyValuePair<Mod, List<VersionedMod>> x) => x.Value.Count);
			KeyValuePair<Mod, List<VersionedMod>> currentMod = modsDependencies.First((KeyValuePair<Mod, List<VersionedMod>> x) => x.Value.Count == minDependenciesCount);
			modsDependencies.Remove(currentMod.Key);
			if (modsDependencies.All((KeyValuePair<Mod, List<VersionedMod>> x) => x.Key.Manifest.Id != currentMod.Key.Manifest.Id))
			{
				foreach (KeyValuePair<Mod, List<VersionedMod>> item in modsDependencies)
				{
					item.Value.RemoveAll((VersionedMod mod) => mod.Id == currentMod.Key.Manifest.Id);
				}
			}
			yield return currentMod.Key;
		}
	}
}
