using System;
using Timberborn.Debugging;
using Timberborn.QuickNotificationSystem;
using UnityEngine;
using UnityEngine.Scripting;

namespace Timberborn.DiagnosticsUI;

internal class GCToggler : IDevModule
{
	private readonly QuickNotificationService _quickNotificationService;

	public GCToggler(QuickNotificationService quickNotificationService)
	{
		_quickNotificationService = quickNotificationService;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Toggle GC", ToggleGC)).Build();
	}

	private void ToggleGC()
	{
		if (Application.isEditor)
		{
			_quickNotificationService.SendNotification("Can't toggle GC in editor");
			return;
		}
		switch (GarbageCollector.GCMode)
		{
		case GarbageCollector.Mode.Enabled:
			GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
			_quickNotificationService.SendNotification("Disabled GC");
			break;
		case GarbageCollector.Mode.Disabled:
			GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
			_quickNotificationService.SendNotification("Enabled GC");
			break;
		default:
			throw new ArgumentOutOfRangeException($"Unexpected GCMode: {GarbageCollector.GCMode}");
		}
	}
}
