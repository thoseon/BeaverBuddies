using System;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.MapEditorTickSystem;
using Timberborn.MapIndexSystem;
using Timberborn.SimulationSystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.TerrainSystemRendering;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.SoilMoistureSystem;

[MapEditorTickable]
internal class SoilMoistureService : ISoilMoistureService, ILoadableSingleton, ITickableSingleton
{
	private readonly SoilMoistureSimulator _soilMoistureSimulator;

	private readonly MapIndexService _mapIndexService;

	private readonly IThreadSafeColumnTerrainMap _threadSafeColumnTerrainMap;

	private readonly IBlockService _blockService;

	private readonly TerrainMaterialMap _terrainMaterialMap;

	private readonly SimulationController _simulationController;

	private readonly ISpecService _specService;

	private float[] _threadSafeMoistureLevels;

	private int _verticalStride;

	private float _maxDesertIntensity;

	private int _desertMoistureThreshold;

	public SoilMoistureService(SoilMoistureSimulator soilMoistureSimulator, MapIndexService mapIndexService, IThreadSafeColumnTerrainMap threadSafeColumnTerrainMap, IBlockService blockService, TerrainMaterialMap terrainMaterialMap, SimulationController simulationController, ISpecService specService)
	{
		_soilMoistureSimulator = soilMoistureSimulator;
		_mapIndexService = mapIndexService;
		_threadSafeColumnTerrainMap = threadSafeColumnTerrainMap;
		_blockService = blockService;
		_terrainMaterialMap = terrainMaterialMap;
		_simulationController = simulationController;
		_specService = specService;
	}

	public void Load()
	{
		_verticalStride = _mapIndexService.VerticalStride;
		SoilMoistureMapSpec singleSpec = _specService.GetSingleSpec<SoilMoistureMapSpec>();
		_maxDesertIntensity = singleSpec.MaxDesertIntensity;
		_desertMoistureThreshold = singleSpec.DesertMoistureThreshold;
		InitializeDesertIntensity();
	}

	public void Tick()
	{
		Array.Resize(ref _threadSafeMoistureLevels, _soilMoistureSimulator.MoistureLevels.Length);
		if (_simulationController.ShouldResetSimulation)
		{
			Reset();
		}
		else
		{
			UpdateMoistureLevels();
		}
	}

	public bool SoilIsMoist(Vector3Int coordinates)
	{
		int index2D = _mapIndexService.CellToIndex(coordinates.XY());
		if (_threadSafeColumnTerrainMap.TryGetIndexAtCeiling(index2D, coordinates.z, out var index3D))
		{
			return _threadSafeMoistureLevels[index3D] > 0f;
		}
		return false;
	}

	public float SoilMoisture(int index)
	{
		return _threadSafeMoistureLevels[index];
	}

	private void InitializeDesertIntensity()
	{
		ReadOnlySpan<float> moistureLevels = _soilMoistureSimulator.MoistureLevels;
		_threadSafeMoistureLevels = new float[moistureLevels.Length];
		moistureLevels.CopyTo(_threadSafeMoistureLevels);
		foreach (int item in _mapIndexService.Indices2D)
		{
			int columnCount = _threadSafeColumnTerrainMap.GetColumnCount(item);
			for (int i = 0; i < columnCount; i++)
			{
				int num = item + i * _verticalStride;
				int columnCeiling = _threadSafeColumnTerrainMap.GetColumnCeiling(num);
				Vector3Int coordinates = _mapIndexService.IndexToCoordinates(item, columnCeiling);
				UpdateDesertIntensity(coordinates, _threadSafeMoistureLevels[num]);
			}
		}
	}

	private void Reset()
	{
		foreach (int item in _mapIndexService.Indices2D)
		{
			int columnCount = _threadSafeColumnTerrainMap.GetColumnCount(item);
			for (int i = 0; i < columnCount; i++)
			{
				int index3D = item + i * _verticalStride;
				int columnCeiling = _threadSafeColumnTerrainMap.GetColumnCeiling(index3D);
				Vector3Int coordinates = _mapIndexService.IndexToCoordinates(item, columnCeiling);
				SetMoistureLevel(coordinates, index3D, 0f);
			}
		}
		_terrainMaterialMap.ResetDesertMap();
	}

	private void UpdateMoistureLevels()
	{
		ReadOnlySpan<float> moistureLevels = _soilMoistureSimulator.MoistureLevels;
		ReadOnlySpan<bool> moistureLevelsChangedLastTick = _soilMoistureSimulator.MoistureLevelsChangedLastTick;
		foreach (int item in _mapIndexService.Indices2D)
		{
			int columnCount = _threadSafeColumnTerrainMap.GetColumnCount(item);
			for (int i = 0; i < columnCount; i++)
			{
				int num = item + i * _verticalStride;
				if (moistureLevelsChangedLastTick[num])
				{
					int columnCeiling = _threadSafeColumnTerrainMap.GetColumnCeiling(num);
					Vector3Int coordinates = _mapIndexService.IndexToCoordinates(item, columnCeiling);
					SetMoistureLevel(coordinates, num, moistureLevels[num]);
				}
			}
		}
	}

	private void SetMoistureLevel(Vector3Int coordinates, int index3D, float newLevel)
	{
		float num = _threadSafeMoistureLevels[index3D];
		if (newLevel > 0f && num <= 0f)
		{
			GetDryObjectAt(coordinates)?.ExitDryState();
		}
		else if (newLevel <= 0f && num > 0f)
		{
			GetDryObjectAt(coordinates)?.EnterDryState();
		}
		UpdateDesertIntensity(coordinates, newLevel);
		_threadSafeMoistureLevels[index3D] = newLevel;
	}

	private DryObject GetDryObjectAt(Vector3Int coordinates)
	{
		return _blockService.GetBottomObjectComponentAt<DryObject>(coordinates);
	}

	private void UpdateDesertIntensity(Vector3Int coordinates, float moistureLevel)
	{
		float desertIntensity;
		if (moistureLevel == 0f)
		{
			desertIntensity = 1f;
		}
		else if (moistureLevel <= (float)_desertMoistureThreshold)
		{
			desertIntensity = 1f - moistureLevel / (float)_desertMoistureThreshold;
			desertIntensity *= _maxDesertIntensity;
		}
		else
		{
			desertIntensity = 0f;
		}
		_terrainMaterialMap.SetDesertIntensity(coordinates, desertIntensity);
	}
}
