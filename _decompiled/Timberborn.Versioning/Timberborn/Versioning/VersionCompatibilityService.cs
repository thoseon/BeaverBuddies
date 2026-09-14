namespace Timberborn.Versioning;

public class VersionCompatibilityService
{
	private static readonly int ForwardCompatibilityDepth = 2;

	private readonly Version _currentVersion;

	private readonly Version _softCapVersion;

	private readonly Version _hardCapVersion;

	public VersionCompatibilityService(Version currentVersion, Version softCapVersion, Version hardCapVersion)
	{
		_currentVersion = currentVersion;
		_softCapVersion = softCapVersion;
		_hardCapVersion = hardCapVersion;
	}

	public bool VersionIsFullyCompatible(Version versionToCheck)
	{
		if (!versionToCheck.IsDevelopmentVersion)
		{
			if (VersionIsForwardCompatible(versionToCheck))
			{
				return VersionIsBackwardCompatible(versionToCheck);
			}
			return false;
		}
		return true;
	}

	public bool VersionIsSemiCompatible(Version versionToCheck)
	{
		return versionToCheck.IsEqualOrHigherThan(_hardCapVersion);
	}

	public bool VersionIsForwardCompatible(Version versionToCheck)
	{
		if (!_currentVersion.IsDevelopmentVersion && !versionToCheck.IsDevelopmentVersion)
		{
			if (!versionToCheck.IsFromSameBranch(_currentVersion))
			{
				return _currentVersion.IsEqualOrHigherThan(versionToCheck);
			}
			return _currentVersion.IsEqualOrHigherThan(versionToCheck, ForwardCompatibilityDepth);
		}
		return true;
	}

	private bool VersionIsBackwardCompatible(Version versionToCheck)
	{
		return versionToCheck.IsEqualOrHigherThan(_softCapVersion);
	}
}
