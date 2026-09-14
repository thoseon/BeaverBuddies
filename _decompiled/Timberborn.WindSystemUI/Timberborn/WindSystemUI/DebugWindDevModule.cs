using Timberborn.Debugging;
using Timberborn.QuickNotificationSystem;
using Timberborn.WindSystem;
using UnityEngine;

namespace Timberborn.WindSystemUI;

internal class DebugWindDevModule : IDevModule
{
	private readonly WindService _windService;

	private readonly QuickNotificationService _quickNotificationService;

	public DebugWindDevModule(WindService windService, QuickNotificationService quickNotificationService)
	{
		_windService = windService;
		_quickNotificationService = quickNotificationService;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Toggle forced wind", ToggleForcedWind)).Build();
	}

	private void ToggleForcedWind()
	{
		_windService.ToggleForcedWind();
		Debug.Log($"Direction: ({_windService.WindDirection.x:F2}, {_windService.WindDirection.y:F2})" + $", Strength: {_windService.WindStrength:F3}");
		_quickNotificationService.SendNotification("Forced wind: " + (_windService.IsForcedWind ? "ENABLED" : "DISABLED"));
	}
}
