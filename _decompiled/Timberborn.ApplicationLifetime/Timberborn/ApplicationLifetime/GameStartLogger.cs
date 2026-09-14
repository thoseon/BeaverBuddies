using System;
using System.Text;
using Timberborn.Modding;
using Timberborn.PlatformUtilities;
using UnityEngine;

namespace Timberborn.ApplicationLifetime;

internal static class GameStartLogger
{
	private static readonly string MachineIdKey = "MachineId";

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void Log()
	{
		if (!Application.isEditor)
		{
			Debug.Log("Starting game version " + Application.version);
			Debug.Log(GetSystemInfo());
			Debug.Log(MachineIdKey + ": " + GetMachineId());
			ExternalModFinder.CheckForMods();
		}
	}

	private static string GetSystemInfo()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("System info:");
		stringBuilder.AppendLine("  System: " + SystemInfo.operatingSystem);
		stringBuilder.AppendLine("  CPU: " + SystemInfo.processorType);
		stringBuilder.AppendLine("  CPU manufacturer: " + SystemInfo.processorManufacturer);
		stringBuilder.AppendLine("  CPU model: " + SystemInfo.processorModel);
		stringBuilder.AppendLine($"  CPU count: {SystemInfo.processorCount}");
		stringBuilder.AppendLine($"  CPU frequency: {SystemInfo.processorFrequency}");
		stringBuilder.AppendLine($"  CPU problematic: {ProblematicProcessorInfo.IsProblematic()}");
		stringBuilder.AppendLine("  CPU microcode: " + ProblematicProcessorInfo.GetMicrocodeVersion());
		stringBuilder.AppendLine("  GPU: " + SystemInfo.graphicsDeviceName);
		stringBuilder.AppendLine($"  GPU memory: {SystemInfo.graphicsMemorySize}MB");
		stringBuilder.AppendLine($"  RAM: {SystemInfo.systemMemorySize}MB");
		return stringBuilder.ToString();
	}

	private static string GetMachineId()
	{
		try
		{
			if (!PlayerPrefs.HasKey(MachineIdKey))
			{
				PlayerPrefs.SetString(MachineIdKey, Guid.NewGuid().ToString());
				PlayerPrefs.Save();
			}
			return PlayerPrefs.GetString(MachineIdKey);
		}
		catch (Exception message)
		{
			Debug.Log(message);
			return "error";
		}
	}
}
