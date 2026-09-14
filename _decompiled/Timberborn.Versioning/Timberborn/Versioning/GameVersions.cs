using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Timberborn.Versioning;

public static class GameVersions
{
	private static readonly string VersionsFileName = "VersionNumbers.json";

	public static Version CurrentVersion { get; } = Version.Create(Application.version);

	public static Version ReadCurrentVersionFromFile()
	{
		return ReadVersionFromFile("CurrentVersion");
	}

	public static Version ReadSoftCapVersionFromFile()
	{
		return ReadVersionFromFile("SoftCapVersion");
	}

	public static Version ReadHardCapSaveVersionFromFile()
	{
		return ReadVersionFromFile("HardCapSaveVersion");
	}

	public static Version ReadHardCapMapVersionFromFile()
	{
		return ReadVersionFromFile("HardCapMapVersion");
	}

	private static Version ReadVersionFromFile(string versionType)
	{
		return Version.Create(JObject.Parse(File.ReadAllText(Path.Combine(Application.streamingAssetsPath, VersionsFileName))).Value<string>(versionType));
	}
}
