using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.Versioning;

namespace Timberborn.Modding;

public class ModManifest
{
	public string Name { get; }

	public string Description { get; }

	public Version Version { get; }

	public string Id { get; }

	public Version MinimumGameVersion { get; }

	public ImmutableArray<VersionedMod> RequiredMods { get; }

	public ImmutableArray<VersionedMod> OptionalMods { get; }

	public ModManifest(string name, string description, Version version, string id, Version minimumGameVersion, IEnumerable<VersionedMod> requiredMods, IEnumerable<VersionedMod> optionalMods)
	{
		Name = name;
		Description = description;
		Version = version;
		Id = id;
		MinimumGameVersion = minimumGameVersion;
		RequiredMods = requiredMods.ToImmutableArray();
		OptionalMods = optionalMods.ToImmutableArray();
	}
}
