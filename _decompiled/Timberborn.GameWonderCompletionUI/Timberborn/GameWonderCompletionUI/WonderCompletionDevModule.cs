using Timberborn.Debugging;
using Timberborn.GameFactionSystem;
using Timberborn.GameWonderCompletion;
using Timberborn.QuickNotificationSystem;

namespace Timberborn.GameWonderCompletionUI;

internal class WonderCompletionDevModule : IDevModule
{
	private readonly GameWonderCompletionService _gameWonderCompletionService;

	private readonly MapNameService _mapNameService;

	private readonly FactionService _factionService;

	private readonly QuickNotificationService _quickNotificationService;

	public WonderCompletionDevModule(GameWonderCompletionService gameWonderCompletionService, MapNameService mapNameService, FactionService factionService, QuickNotificationService quickNotificationService)
	{
		_gameWonderCompletionService = gameWonderCompletionService;
		_mapNameService = mapNameService;
		_factionService = factionService;
		_quickNotificationService = quickNotificationService;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Wonders: Complete on this map", CompleteWonder)).AddMethod(DevMethod.Create("Wonders: Revoke all completions", RevokeAllCompletions)).Build();
	}

	private void CompleteWonder()
	{
		_gameWonderCompletionService.CompleteWonder();
		string text = (_mapNameService.HasMapName ? ("Map " + _mapNameService.Name + " completed for faction " + _factionService.Current.Id) : "Unable to complete - missing map name");
		_quickNotificationService.SendNotification(text);
	}

	private void RevokeAllCompletions()
	{
		_gameWonderCompletionService.RevokeWonderCompletionForAllFactions();
		string text = (_mapNameService.HasMapName ? ("Completion of map " + _mapNameService.Name + " revoked for all factions") : "Unable to complete - missing map name");
		_quickNotificationService.SendNotification(text);
	}
}
