using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Buildings;
using Timberborn.MapStateSystem;
using Timberborn.Persistence;
using Timberborn.PlayerDataSystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.ScienceSystem;

public class BuildingUnlockingService : ISaveableSingleton, ILoadableSingleton
{
	private static readonly SingletonKey BuildingUnlockingServiceKey = new SingletonKey("BuildingUnlockingService");

	private static readonly ListKey<string> UnlockedBuildingsKey = new ListKey<string>("UnlockedBuildings");

	private static readonly string UnlockedOncePlayerDataKey = "UnlockedOnce_";

	private readonly ISingletonLoader _singletonLoader;

	private readonly ScienceService _scienceService;

	private readonly BuildingService _buildingService;

	private readonly MapEditorMode _mapEditorMode;

	private readonly EventBus _eventBus;

	private readonly TemplateNameMapper _templateNameMapper;

	private readonly IPlayerDataService _playerDataService;

	private readonly HashSet<string> _unlockedBuildings = new HashSet<string>();

	public BuildingUnlockingService(ISingletonLoader singletonLoader, ScienceService scienceService, BuildingService buildingService, MapEditorMode mapEditorMode, EventBus eventBus, TemplateNameMapper templateNameMapper, IPlayerDataService playerDataService)
	{
		_singletonLoader = singletonLoader;
		_scienceService = scienceService;
		_buildingService = buildingService;
		_mapEditorMode = mapEditorMode;
		_eventBus = eventBus;
		_templateNameMapper = templateNameMapper;
		_playerDataService = playerDataService;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (!_mapEditorMode.IsMapEditor)
		{
			singletonSaver.GetSingleton(BuildingUnlockingServiceKey).Set(UnlockedBuildingsKey, _unlockedBuildings);
		}
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(BuildingUnlockingServiceKey, out var objectLoader))
		{
			foreach (string item in objectLoader.Get(UnlockedBuildingsKey))
			{
				if (_templateNameMapper.TryGetTemplate(item, out var templateSpec))
				{
					_unlockedBuildings.Add(templateSpec.TemplateName);
				}
			}
		}
		LoadUnlockableOnce();
	}

	public bool Unlocked(BuildingSpec buildingSpec)
	{
		if (buildingSpec.ScienceCost != 0)
		{
			return _unlockedBuildings.Contains(_buildingService.GetTemplateName(buildingSpec));
		}
		return true;
	}

	public void Unlock(BuildingSpec buildingSpec)
	{
		if (!Unlockable(buildingSpec))
		{
			throw new ArgumentException("Can't unlock " + _buildingService.GetTemplateName(buildingSpec) + ", not enough science points!");
		}
		_scienceService.SubtractPoints(buildingSpec.ScienceCost);
		UnlockIgnoringCost(buildingSpec);
	}

	public void UnlockIgnoringCost(BuildingSpec buildingSpec)
	{
		_unlockedBuildings.Add(_buildingService.GetTemplateName(buildingSpec));
		if (buildingSpec.HasSpec<UnlockableOnceSpec>())
		{
			_playerDataService.SetBool(UnlockedOnceKey(buildingSpec), value: true);
		}
		_eventBus.Post(new BuildingUnlockedEvent(buildingSpec));
	}

	public bool Unlockable(BuildingSpec buildingSpec)
	{
		return _scienceService.SciencePoints >= buildingSpec.ScienceCost;
	}

	private void LoadUnlockableOnce()
	{
		foreach (BuildingSpec item in _buildingService.Buildings.Where((BuildingSpec building) => building.HasSpec<UnlockableOnceSpec>()))
		{
			if (_playerDataService.GetBool(UnlockedOnceKey(item), defaultValue: false))
			{
				_unlockedBuildings.Add(item.GetSpec<TemplateSpec>().TemplateName);
			}
		}
	}

	private static string UnlockedOnceKey(BuildingSpec buildingSpec)
	{
		return UnlockedOncePlayerDataKey + buildingSpec.GetSpec<TemplateSpec>().TemplateName;
	}
}
