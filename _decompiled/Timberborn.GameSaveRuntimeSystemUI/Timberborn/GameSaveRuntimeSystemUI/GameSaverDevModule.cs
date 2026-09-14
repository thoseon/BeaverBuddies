using System;
using System.Linq;
using Timberborn.Debugging;
using Timberborn.GameSaveRuntimeSystem;
using Timberborn.QuickNotificationSystem;
using UnityEngine;

namespace Timberborn.GameSaveRuntimeSystemUI;

internal class GameSaverDevModule : IDevModule
{
	private static readonly int SaveCount = 20;

	private readonly GameSaver _gameSaver;

	private readonly QuickNotificationService _quickNotificationService;

	public GameSaverDevModule(GameSaver gameSaver, QuickNotificationService quickNotificationService)
	{
		_gameSaver = gameSaver;
		_quickNotificationService = quickNotificationService;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create($"Save {SaveCount}x to memory", Save)).Build();
	}

	private void Save()
	{
		double num = _gameSaver.BenchmarkSavingToMemory(SaveCount).Average((TimeSpan timeSpan) => timeSpan.TotalSeconds);
		string text = $"Saved {SaveCount}x to memory in an average of {num:0.00}s";
		_quickNotificationService.SendNotification(text);
		Debug.Log(text);
	}
}
