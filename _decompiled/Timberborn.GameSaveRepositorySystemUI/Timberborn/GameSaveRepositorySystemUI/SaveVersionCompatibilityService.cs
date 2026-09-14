using Timberborn.SingletonSystem;
using Timberborn.Versioning;

namespace Timberborn.GameSaveRepositorySystemUI;

public class SaveVersionCompatibilityService : ILoadableSingleton
{
	private VersionCompatibilityService _versionCompatibilityService;

	public void Load()
	{
		_versionCompatibilityService = new VersionCompatibilityService(GameVersions.CurrentVersion, GameVersions.ReadSoftCapVersionFromFile(), GameVersions.ReadHardCapSaveVersionFromFile());
	}

	public bool VersionIsFullyCompatible(Version saveVersion)
	{
		return _versionCompatibilityService.VersionIsFullyCompatible(saveVersion);
	}

	public bool VersionIsSemiCompatible(Version saveVersion)
	{
		return _versionCompatibilityService.VersionIsSemiCompatible(saveVersion);
	}

	public bool VersionIsForwardCompatible(Version saveVersion)
	{
		return _versionCompatibilityService.VersionIsForwardCompatible(saveVersion);
	}
}
