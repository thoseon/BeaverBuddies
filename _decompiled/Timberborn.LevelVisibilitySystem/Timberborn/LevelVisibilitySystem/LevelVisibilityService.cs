using System;
using Timberborn.MapStateSystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using UnityEngine;
using UnityEngine.Rendering;

namespace Timberborn.LevelVisibilitySystem;

internal class LevelVisibilityService : ILevelVisibilityService, IPostLoadableSingleton, ILoadableSingleton
{
	private static readonly int MaxVisibleLevelProperty = Shader.PropertyToID("_MaxVisibleLevel");

	private static readonly GlobalKeyword UseLevelVisibilityKey = GlobalKeyword.Create("_USE_LEVEL_VISIBILITY");

	private readonly EventBus _eventBus;

	private readonly ITerrainService _terrainService;

	private readonly MapSize _mapSize;

	private int _maxLevelWithHidableObjects;

	private int _maxLevelHidingAnything;

	private bool _isLoaded;

	public int MaxVisibleLevel { get; private set; }

	public bool LevelIsAtMin => MaxVisibleLevel == 0;

	public bool LevelIsAtMax => MaxVisibleLevel == MaxVisibleLevelLimit;

	public bool TerrainLevelIsAtMax => MaxVisibleLevel == MaxVisibleTerrainLevelLimit;

	private int MaxVisibleLevelLimit => _mapSize.TotalSize.z;

	private int MaxVisibleTerrainLevelLimit => _mapSize.TerrainSize.z;

	public event EventHandler<int> MaxVisibleLevelChanged;

	public LevelVisibilityService(EventBus eventBus, ITerrainService terrainService, MapSize mapSize)
	{
		_eventBus = eventBus;
		_terrainService = terrainService;
		_mapSize = mapSize;
	}

	public void Load()
	{
		MaxVisibleLevel = _mapSize.MaxGameTerrainHeight + _mapSize.MaxHeightAboveTerrain;
		SetLevelsWithAnythingHidable(0);
		_terrainService.MinMaxTerrainHeightChanged += delegate
		{
			UpdateMaxLevelHidingAnything();
		};
	}

	public void PostLoad()
	{
		_isLoaded = true;
		ResetMaxVisibleLevel();
		UpdateMaxLevelHidingAnything();
	}

	public void SetMaxVisibleLevel(int newMaxVisibleLevel)
	{
		if (MaxVisibleLevel != newMaxVisibleLevel)
		{
			int newMaxVisibleLevel2 = ClampMaxVisibleLevel(newMaxVisibleLevel);
			InternalSetMaxVisibleLevel(newMaxVisibleLevel2);
		}
	}

	public void ResetMaxVisibleLevel()
	{
		InternalSetMaxVisibleLevel(Math.Max(MaxVisibleLevelLimit, MaxVisibleTerrainLevelLimit));
	}

	public bool BlockIsVisible(Vector3Int coordinates)
	{
		return coordinates.z <= MaxVisibleLevel;
	}

	public void SetLevelsWithAnythingHidable(int maxLevel)
	{
		_maxLevelWithHidableObjects = maxLevel;
		UpdateMaxLevelHidingAnything();
	}

	private void InternalSetMaxVisibleLevel(int newMaxVisibleLevel)
	{
		int maxVisibleLevel = MaxVisibleLevel;
		if (_isLoaded && maxVisibleLevel != newMaxVisibleLevel)
		{
			MaxVisibleLevel = newMaxVisibleLevel;
			Shader.SetGlobalFloat(MaxVisibleLevelProperty, MaxVisibleLevel);
			Shader.SetKeyword(in UseLevelVisibilityKey, !LevelIsAtMax);
			MaxVisibleLevelChanged?.Invoke(this, MaxVisibleLevel);
			_eventBus.Post(new MaxVisibleLevelChangedEvent(maxVisibleLevel));
		}
	}

	private void UpdateMaxLevelHidingAnything()
	{
		_maxLevelHidingAnything = Math.Max(0, Math.Max(_terrainService.MaxTerrainHeight - 1, _maxLevelWithHidableObjects - 1));
		if (!LevelIsAtMax && MaxVisibleLevel > _maxLevelHidingAnything)
		{
			ResetMaxVisibleLevel();
		}
	}

	private int ClampMaxVisibleLevel(int maxVisibleLevel)
	{
		if (maxVisibleLevel > MaxVisibleLevelLimit)
		{
			return MaxVisibleLevelLimit;
		}
		if (maxVisibleLevel < 0)
		{
			return 0;
		}
		if (maxVisibleLevel > _maxLevelHidingAnything)
		{
			if (maxVisibleLevel <= MaxVisibleLevel)
			{
				return _maxLevelHidingAnything;
			}
			return MaxVisibleLevelLimit;
		}
		return maxVisibleLevel;
	}
}
