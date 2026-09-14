using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Timberborn.ErrorReporting;

public static class PlayerLogCleaner
{
	private static readonly List<Regex> SafePatterns = new List<Regex>
	{
		new Regex("^Mono path\\[0\\] = "),
		new Regex("^Mono config path = "),
		new Regex("^Input System module state changed to: "),
		new Regex("^Initialize engine version: "),
		new Regex("^\\[Subsystems\\] Discovering subsystems at path "),
		new Regex("^GfxDevice: creating device client;"),
		new Regex("^\\[D3D12 Device Filter\\]"),
		new Regex("^Direct3D:$"),
		new Regex("^    Version: "),
		new Regex("^    Renderer: "),
		new Regex("^    Vendor: "),
		new Regex("^    Driver: "),
		new Regex("^    VRAM: "),
		new Regex("^Begin MonoManager ReloadAssembly"),
		new Regex("^- Loaded All Assemblies, in "),
		new Regex("^- Finished resetting the current domain, in "),
		new Regex("^<RI> Initializing input\\.$"),
		new Regex("^Using Windows\\.Gaming\\.Input"),
		new Regex("^UnloadTime: \\d+\\.\\d+ ms"),
		new Regex("^Total: \\d+\\.\\d+ ms \\(FindLiveObjects: \\d+\\.\\d+ ms CreateObjectMapping: \\d+\\.\\d+ ."),
		new Regex("^Starting game version."),
		new Regex("^\\[Physics::Module\\]"),
		new Regex("^System info:$"),
		new Regex("^  System:"),
		new Regex("^  CPU:"),
		new Regex("^  CPU manufacturer:"),
		new Regex("^  CPU model:"),
		new Regex("^  CPU count:"),
		new Regex("^  CPU frequency:"),
		new Regex("^  CPU problematic:"),
		new Regex("^  CPU microcode:"),
		new Regex("^  GPU:"),
		new Regex("^  GPU memory:"),
		new Regex("^  RAM:"),
		new Regex("^MachineId:"),
		new Regex("^Successfully connected to the Steam client\\."),
		new Regex("^Unloading \\d+ Unused Serialized files \\(Serialized files now loaded: \\d+\\)"),
		new Regex("^Unloading \\d+ unused Assets to reduce memory usage\\. Loaded Objects now: \\d+\\."),
		new Regex("^Application focus gained at"),
		new Regex("^Application focus lost at"),
		new Regex("^Previous resolution: "),
		new Regex("^New resolution "),
		new Regex("^Display resolution: "),
		new Regex("^Full screen: "),
		new Regex("^Resolution scale: "),
		new Regex("^VSync count: "),
		new Regex("^Brightness: "),
		new Regex("^Frame rate limit: "),
		new Regex("^RenderGraph is now enabled\\.$"),
		new Regex("^Dev mode enabled$"),
		new Regex("^Starting new game at"),
		new Regex("^FactionId: .*, MapFileReference: Name: .*, Path: , Resource: .*"),
		new Regex("^StartingAdults:"),
		new Regex("^AdultAgeProgress:"),
		new Regex("^StartingChildren:"),
		new Regex("^ChildAgeProgress:"),
		new Regex("^FoodConsumption:"),
		new Regex("^WaterConsumption:"),
		new Regex("^StartingFood:"),
		new Regex("^StartingWater:"),
		new Regex("^TemperateWeatherDuration:"),
		new Regex("^DroughtDuration:"),
		new Regex("^DroughtDurationHandicapMultiplier:"),
		new Regex("^DroughtDurationHandicapCycles:"),
		new Regex("^CyclesBeforeRandomizingBadtide:"),
		new Regex("^ChanceForBadtide:"),
		new Regex("^BadtideDuration:"),
		new Regex("^BadtideDurationHandicapMultiplier:"),
		new Regex("^BadtideDurationHandicapCycles:"),
		new Regex("^InjuryChance:"),
		new Regex("^DemolishableRecoveryRate:"),
		new Regex("^DisplayNameLocKey:"),
		new Regex("^DescriptionLocKey:"),
		new Regex("^Number of threads: "),
		new Regex("^Load time: \\d+ms \\(scene index: \\d\\)"),
		new Regex("^Initialized SettlementReference to "),
		new Regex("^Saving game to "),
		new Regex("^Saved game in "),
		new Regex("^Input System polling thread exited\\."),
		new Regex("^Loading saved game "),
		new Regex("^Opening file: "),
		new Regex("^Deleted excess autosave "),
		new Regex("^Saving map to "),
		new Regex("^Loading map from Name: "),
		new Regex("^Creating new \\d+x\\d+ map at"),
		new Regex("^Debug mode enabled")
	};

	public static string GetCleanedPlayerLog()
	{
		string text = Path.Combine(Application.temporaryCachePath, "TimberbornTemporaryLog");
		File.Copy(Application.consoleLogPath, text, overwrite: true);
		string text2 = "No entries in the log file";
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string item in File.ReadLines(text))
		{
			text2 = item;
			bool flag = false;
			foreach (Regex safePattern in SafePatterns)
			{
				if (string.IsNullOrWhiteSpace(item) || safePattern.IsMatch(item))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				stringBuilder.AppendLine(item);
			}
		}
		stringBuilder.AppendLine("\n\n Last line: " + text2);
		return stringBuilder.ToString();
	}
}
