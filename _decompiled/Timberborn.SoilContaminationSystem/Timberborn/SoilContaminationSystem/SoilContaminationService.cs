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

namespace Timberborn.SoilContaminationSystem;

[MapEditorTickable]
internal class SoilContaminationService : ISoilContaminationService, ILoadableSingleton, ITickableSingleton
{
	private readonly SoilContaminationSimulator _soilContaminationSimulator;

	private readonly MapIndexService _mapIndexService;

	private readonly IThreadSafeColumnTerrainMap _threadSafeColumnTerrainMap;

	private readonly TerrainMaterialMap _terrainMaterialMap;

	private readonly IBlockService _blockService;

	private readonly ISpecService _specService;

	private readonly SimulationController _simulationController;

	private readonly ITerrainService _terrainService;

	private float[] _threadSafeContaminationLevels;

	private int _verticalStride;

	private float _maxMapContamination;

	private float _contaminationThreshold;

	public SoilContaminationService(SoilContaminationSimulator soilContaminationSimulator, MapIndexService mapIndexService, IThreadSafeColumnTerrainMap threadSafeColumnTerrainMap, TerrainMaterialMap terrainMaterialMap, IBlockService blockService, ISpecService specService, SimulationController simulationController, ITerrainService terrainService)
	{
		_soilContaminationSimulator = soilContaminationSimulator;
		_mapIndexService = mapIndexService;
		_threadSafeColumnTerrainMap = threadSafeColumnTerrainMap;
		_terrainMaterialMap = terrainMaterialMap;
		_blockService = blockService;
		_specService = specService;
		_simulationController = simulationController;
		_terrainService = terrainService;
	}

	public void Load()
	{
		_verticalStride = _mapIndexService.VerticalStride;
		SoilContaminationMapSpec singleSpec = _specService.GetSingleSpec<SoilContaminationMapSpec>();
		_maxMapContamination = singleSpec.MaxMapContamination;
		_contaminationThreshold = singleSpec.ContaminationThreshold;
		InitializeContamination();
	}

	public void Tick()
	{
		int length = _soilContaminationSimulator.ContaminationLevels.Length;
		Array.Resize(ref _threadSafeContaminationLevels, length);
		if (_simulationController.ShouldResetSimulation)
		{
			Reset();
		}
		else
		{
			UpdateContaminationLevels();
		}
	}

	public float Contamination(int index)
	{
		return _threadSafeContaminationLevels[index];
	}

	public bool SoilIsContaminated(Vector3Int coordinates)
	{
		int index2D = _mapIndexService.CellToIndex(coordinates.XY());
		if (_threadSafeColumnTerrainMap.TryGetIndexAtCeiling(index2D, coordinates.z, out var index3D))
		{
			return _threadSafeContaminationLevels[index3D] > 0f;
		}
		return false;
	}

	private void InitializeContamination()
	{
		ReadOnlySpan<float> contaminationLevels = _soilContaminationSimulator.ContaminationLevels;
		_threadSafeContaminationLevels = new float[contaminationLevels.Length];
		contaminationLevels.CopyTo(_threadSafeContaminationLevels);
		foreach (int item in _mapIndexService.Indices2D)
		{
			int columnCount = _terrainService.GetColumnCount(item);
			for (int i = 0; i < columnCount; i++)
			{
				int num = item + i * _verticalStride;
				int columnCeiling = _terrainService.GetColumnCeiling(num);
				Vector3Int coordinates = _mapIndexService.IndexToCoordinates(item, columnCeiling);
				UpdateContamination(coordinates, _threadSafeContaminationLevels[num]);
			}
		}
	}

	private void Reset()
	{
		foreach (int item in _mapIndexService.Indices2D)
		{
			int columnCount = _terrainService.GetColumnCount(item);
			for (int i = 0; i < columnCount; i++)
			{
				int index3D = item + i * _verticalStride;
				int columnCeiling = _terrainService.GetColumnCeiling(index3D);
				Vector3Int coordinates = _mapIndexService.IndexToCoordinates(item, columnCeiling);
				SetContaminationLevel(coordinates, index3D, 0f);
			}
		}
		_terrainMaterialMap.ResetContaminationMap();
	}

	private void UpdateContaminationLevels()
	{
		ReadOnlySpan<bool> contaminationsChangedLastTick = _soilContaminationSimulator.ContaminationsChangedLastTick;
		ReadOnlySpan<float> contaminationLevels = _soilContaminationSimulator.ContaminationLevels;
		foreach (int item in _mapIndexService.Indices2D)
		{
			int columnCount = _terrainService.GetColumnCount(item);
			for (int i = 0; i < columnCount; i++)
			{
				int num = item + i * _verticalStride;
				if (contaminationsChangedLastTick[num])
				{
					int columnCeiling = _terrainService.GetColumnCeiling(num);
					Vector3Int coordinates = _mapIndexService.IndexToCoordinates(item, columnCeiling);
					SetContaminationLevel(coordinates, num, contaminationLevels[num]);
				}
			}
		}
	}

	private void SetContaminationLevel(Vector3Int coordinates, int index3D, float newLevel)
	{
		float num = _threadSafeContaminationLevels[index3D];
		if (newLevel > 0f && num <= 0f)
		{
			GetContaminatedObjectAt(coordinates)?.EnterContaminatedState();
		}
		else if (newLevel <= 0f && num > 0f)
		{
			GetContaminatedObjectAt(coordinates)?.ExitContaminatedState();
		}
		UpdateContamination(coordinates, newLevel);
		_threadSafeContaminationLevels[index3D] = newLevel;
	}

	private ContaminatedObject GetContaminatedObjectAt(Vector3Int coordinates)
	{
		return _blockService.GetBottomObjectComponentAt<ContaminatedObject>(coordinates);
	}

	private void UpdateContamination(Vector3Int coordinates, float contamination)
	{
		_terrainMaterialMap.SetSoilContamination(coordinates, GetMapSoilContamination(contamination));
	}

	private float GetMapSoilContamination(float contamination)
	{
		if (contamination > 0f)
		{
			if (contamination <= _contaminationThreshold)
			{
				float num = 1f - contamination / _contaminationThreshold;
				return 1f - _maxMapContamination * num;
			}
			return 1f;
		}
		return 0f;
	}
}
