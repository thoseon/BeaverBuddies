using Timberborn.Debugging;
using Timberborn.QuickNotificationSystem;

namespace Timberborn.MortalSystem;

public class LongLastingCorpsesDevModule : IDevModule
{
	private readonly LongLastingCorpsesService _longLastingCorpsesService;

	private readonly QuickNotificationService _quickNotificationService;

	public LongLastingCorpsesDevModule(LongLastingCorpsesService longLastingCorpsesService, QuickNotificationService quickNotificationService)
	{
		_longLastingCorpsesService = longLastingCorpsesService;
		_quickNotificationService = quickNotificationService;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Toggle long lasting corpses", ToggleLongLastingCorpses)).Build();
	}

	private void ToggleLongLastingCorpses()
	{
		_longLastingCorpsesService.Toggle();
		_quickNotificationService.SendNotification("Long lasting corpses " + (_longLastingCorpsesService.Enabled ? "enabled" : "disabled"));
	}
}
