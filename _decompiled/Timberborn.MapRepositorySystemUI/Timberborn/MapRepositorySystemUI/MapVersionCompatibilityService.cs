using Timberborn.MapRepositorySystem;
using Timberborn.SingletonSystem;
using Timberborn.Versioning;
using Timberborn.VersioningSerialization;

namespace Timberborn.MapRepositorySystemUI;

public class MapVersionCompatibilityService : ILoadableSingleton
{
	private readonly MapDeserializer _mapDeserializer;

	private readonly VersionSerializer _versionSerializer;

	private VersionCompatibilityService _versionCompatibilityService;

	public MapVersionCompatibilityService(MapDeserializer mapDeserializer, VersionSerializer versionSerializer)
	{
		_mapDeserializer = mapDeserializer;
		_versionSerializer = versionSerializer;
	}

	public void Load()
	{
		_versionCompatibilityService = new VersionCompatibilityService(GameVersions.CurrentVersion, GameVersions.ReadSoftCapVersionFromFile(), GameVersions.ReadHardCapMapVersionFromFile());
	}

	public bool IsMapFullyCompatible(MapFileReference mapFileReference)
	{
		Version mapVersionNumber = GetMapVersionNumber(mapFileReference);
		return VersionIsFullyCompatible(mapVersionNumber);
	}

	public Version GetMapVersionNumber(MapFileReference mapFileReference)
	{
		return _mapDeserializer.ReadFromMapFileUnsafe(mapFileReference, _versionSerializer);
	}

	public bool VersionIsFullyCompatible(Version mapVersion)
	{
		return _versionCompatibilityService.VersionIsFullyCompatible(mapVersion);
	}

	public bool VersionIsSemiCompatible(Version mapVersion)
	{
		return _versionCompatibilityService.VersionIsSemiCompatible(mapVersion);
	}
}
