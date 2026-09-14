using System.Linq;
using Timberborn.GameFactionSystem;
using Timberborn.WonderCompletion;

namespace Timberborn.GameWonderCompletion;

public class GameWonderCompletionService
{
	private readonly MapNameService _mapNameService;

	private readonly FactionService _factionService;

	private readonly WonderCompletionService _wonderCompletionService;

	public bool WasCompletedFirstTimeForMap { get; private set; }

	public bool WasCompletedFirstTimeForFaction { get; private set; }

	public GameWonderCompletionService(MapNameService mapNameService, FactionService factionService, WonderCompletionService wonderCompletionService)
	{
		_mapNameService = mapNameService;
		_factionService = factionService;
		_wonderCompletionService = wonderCompletionService;
	}

	public bool IsWonderCompletedWithAnyFaction()
	{
		return _wonderCompletionService.IsWonderCompletedWithAnyFaction(_mapNameService.Name, _mapNameService.IsResource);
	}

	public void CompleteWonder()
	{
		if (_mapNameService.HasMapName && !IsWonderCompletedWithCurrentFaction())
		{
			bool flag = (WasCompletedFirstTimeForFaction = IsWonderCompletedWithAnyFaction());
			WasCompletedFirstTimeForMap = !flag;
			_wonderCompletionService.CompleteWonder(_mapNameService.Name, _mapNameService.IsResource, _factionService.Current.Id);
		}
	}

	public void RevokeWonderCompletionForAllFactions()
	{
		_wonderCompletionService.RevokeWonderCompletionForAllFactions(_mapNameService.Name, _mapNameService.IsResource);
	}

	private bool IsWonderCompletedWithCurrentFaction()
	{
		return _wonderCompletionService.GetWonderCompletionFactionIds(_mapNameService.Name, _mapNameService.IsResource).Contains(_factionService.Current.Id);
	}
}
