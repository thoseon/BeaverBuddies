using System;
using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.PlayerDataSystem;

namespace Timberborn.WonderCompletion;

public class WonderCompletionService
{
	private static readonly string ResourceMapPrefix = "Resource_";

	private static readonly string DataPrefix = "CompletedWonders_";

	private static readonly string Separator = ";";

	private readonly IPlayerDataService _playerDataService;

	private readonly HashSet<string> _factionIdCache = new HashSet<string>();

	public WonderCompletionService(IPlayerDataService playerDataService)
	{
		_playerDataService = playerDataService;
	}

	public bool IsWonderCompletedWithAnyFaction(string mapName, bool isResource)
	{
		return _playerDataService.HasKey(GetKey(mapName, isResource));
	}

	public IEnumerable<string> GetWonderCompletionFactionIds(string mapName, bool isResource)
	{
		string key = GetKey(mapName, isResource);
		return _playerDataService.GetString(key, string.Empty).Split(Separator, StringSplitOptions.RemoveEmptyEntries);
	}

	public void CompleteWonder(string mapName, bool isResource, string factionId)
	{
		string key = GetKey(mapName, isResource);
		string text = _playerDataService.GetString(key, string.Empty);
		_factionIdCache.AddRange(text.Split(Separator, StringSplitOptions.RemoveEmptyEntries));
		_factionIdCache.Add(factionId);
		_playerDataService.SetString(key, string.Join(Separator, _factionIdCache));
		_factionIdCache.Clear();
	}

	public void RevokeWonderCompletionForAllFactions(string mapName, bool isResource)
	{
		_playerDataService.Remove(GetKey(mapName, isResource));
	}

	private static string GetKey(string mapName, bool isResource)
	{
		string text = (isResource ? ResourceMapPrefix : string.Empty);
		return DataPrefix + text + mapName;
	}
}
