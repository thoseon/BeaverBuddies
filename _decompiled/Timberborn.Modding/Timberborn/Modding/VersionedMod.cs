using Timberborn.Versioning;

namespace Timberborn.Modding;

public class VersionedMod
{
	public string Id { get; }

	public Version MinimumVersion { get; }

	public VersionedMod(string id, Version minimumVersion)
	{
		Id = id;
		MinimumVersion = minimumVersion;
	}
}
