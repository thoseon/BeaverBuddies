using System;
using UnityEngine;

namespace Timberborn.LevelVisibilitySystem;

public interface ILevelVisibilityService
{
	int MaxVisibleLevel { get; }

	bool LevelIsAtMin { get; }

	bool LevelIsAtMax { get; }

	bool TerrainLevelIsAtMax { get; }

	event EventHandler<int> MaxVisibleLevelChanged;

	void SetMaxVisibleLevel(int newMaxVisibleLevel);

	void ResetMaxVisibleLevel();

	bool BlockIsVisible(Vector3Int coordinates);

	void SetLevelsWithAnythingHidable(int maxLevel);
}
