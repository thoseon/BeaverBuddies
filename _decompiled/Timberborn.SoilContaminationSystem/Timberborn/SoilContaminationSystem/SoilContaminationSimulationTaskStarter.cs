using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.MapIndexSystem;
using Timberborn.Multithreading;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.TickSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.SoilContaminationSystem;

internal class SoilContaminationSimulationTaskStarter : ILoadableSingleton
{
	private readonly IParallelizer _parallelizer;

	private readonly MapIndexService _mapIndexService;

	private readonly ISpecService _specService;

	private readonly WaterColumnRetriever _waterColumnRetriever;

	private readonly CeilingRetriever _ceilingRetriever;

	private readonly ITickService _tickService;

	private Vector3Int _mapSize;

	private int _stride;

	private int _verticalStride;

	private float _maximumSoilContamination;

	private float _regularSpreadCost;

	private float _diagonalSpreadCost;

	private float _contaminationDecayRate;

	private float _contaminationSpreadingRate;

	private float _minimumWaterContamination;

	private float _contaminationScaler;

	private float _verticalCostModifier;

	private float _contaminationThreshold;

	private float _contaminationPositiveEqualizationRate;

	private float _contaminationNegativeEqualizationRate;

	public SoilContaminationSimulationTaskStarter(IParallelizer parallelizer, MapIndexService mapIndexService, ISpecService specService, WaterColumnRetriever waterColumnRetriever, CeilingRetriever ceilingRetriever, ITickService tickService)
	{
		_parallelizer = parallelizer;
		_mapIndexService = mapIndexService;
		_specService = specService;
		_waterColumnRetriever = waterColumnRetriever;
		_ceilingRetriever = ceilingRetriever;
		_tickService = tickService;
	}

	public void Load()
	{
		_mapSize = _mapIndexService.TerrainSize;
		_stride = _mapIndexService.Stride;
		_verticalStride = _mapIndexService.VerticalStride;
		SoilContaminationSimulatorSpec singleSpec = _specService.GetSingleSpec<SoilContaminationSimulatorSpec>();
		float tickIntervalInSeconds = _tickService.TickIntervalInSeconds;
		_maximumSoilContamination = 1f - singleSpec.ContaminationThreshold;
		_regularSpreadCost = 1f / (float)singleSpec.MaxRangeFromSource;
		_diagonalSpreadCost = Mathf.Sqrt(2f) / (float)singleSpec.MaxRangeFromSource;
		_contaminationDecayRate = singleSpec.ContaminationDecayRate * tickIntervalInSeconds;
		_contaminationSpreadingRate = singleSpec.ContaminationSpreadingRate * tickIntervalInSeconds;
		_minimumWaterContamination = singleSpec.MinimumWaterContamination;
		_contaminationScaler = 1f / (1f - singleSpec.MinimumWaterContamination);
		_verticalCostModifier = singleSpec.VerticalSpreadCostMultiplier / (float)singleSpec.MaxRangeFromSource;
		_contaminationThreshold = singleSpec.ContaminationThreshold;
		_contaminationPositiveEqualizationRate = singleSpec.ContaminationPositiveEqualizationRate * tickIntervalInSeconds;
		_contaminationNegativeEqualizationRate = singleSpec.ContaminationNegativeEqualizationRate * tickIntervalInSeconds;
	}

	public void StartTask(float[] contaminationLevels, bool[] contaminationsChangedLastTick, float[] contaminationCandidates, float[] lastTickContaminationCandidates, in ReadOnlyArray<byte> waterColumnCounts, in ReadOnlyArray<ReadOnlyWaterColumn> waterColumns, in ReadOnlyArray<byte> terrainColumnCounts, in ReadOnlyArray<ReadOnlyTerrainColumn> terrainColumns, in ReadOnlyArray<bool> contaminationBarriers, in ReadOnlyArray<bool> aboveMoistureBarriers)
	{
		ContaminationDataPreparationTask task = new ContaminationDataPreparationTask(contaminationCandidates, lastTickContaminationCandidates, contaminationsChangedLastTick);
		ContaminationCandidatesCountingTask task2 = new ContaminationCandidatesCountingTask(_waterColumnRetriever, _ceilingRetriever, contaminationCandidates, waterColumnCounts, waterColumns, terrainColumnCounts, terrainColumns, contaminationBarriers, aboveMoistureBarriers, new ReadOnlyArray<float>(lastTickContaminationCandidates), _mapSize.x, _stride, _verticalStride, _maximumSoilContamination, _regularSpreadCost, _diagonalSpreadCost, _contaminationDecayRate, _contaminationSpreadingRate, _minimumWaterContamination, _contaminationScaler, _verticalCostModifier);
		ContaminationsUpdateTask task3 = new ContaminationsUpdateTask(contaminationLevels, contaminationsChangedLastTick, terrainColumnCounts, new ReadOnlyArray<float>(contaminationCandidates), _mapSize.x, _stride, _verticalStride, _contaminationThreshold, _contaminationPositiveEqualizationRate, _contaminationNegativeEqualizationRate);
		_parallelizer.Schedule(in task).ContinueWith(0, _mapSize.y, 1, in task2).ContinueWith(0, _mapSize.y, 5, in task3);
	}
}
