using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.MapIndexSystem;
using Timberborn.Multithreading;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.TickSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.SoilMoistureSystem;

internal class SoilMoistureSimulationTaskStarter : ILoadableSingleton
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

	private float _moistureDecayRate;

	private float _moistureSpreadingRate;

	private float _minimumWaterContamination;

	private int _verticalSpreadCostMultiplier;

	private float _waterContaminationScaler;

	private int _maxClusterSaturation;

	private float[] _saturationToEvaporationMap;

	public SoilMoistureSimulationTaskStarter(IParallelizer parallelizer, MapIndexService mapIndexService, ISpecService specService, WaterColumnRetriever waterColumnRetriever, CeilingRetriever ceilingRetriever, ITickService tickService)
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
		SoilMoistureSimulatorSpec singleSpec = _specService.GetSingleSpec<SoilMoistureSimulatorSpec>();
		float tickIntervalInSeconds = _tickService.TickIntervalInSeconds;
		_moistureDecayRate = singleSpec.MoistureDecayRate * tickIntervalInSeconds;
		_moistureSpreadingRate = singleSpec.MoistureSpreadingRate * tickIntervalInSeconds;
		_minimumWaterContamination = singleSpec.MinimumWaterContamination;
		_verticalSpreadCostMultiplier = singleSpec.VerticalSpreadCostMultiplier;
		_waterContaminationScaler = 1f / singleSpec.MaximumWaterContamination;
		_maxClusterSaturation = singleSpec.MaxClusterSaturation;
		InitializeEvaporationModifiers(singleSpec);
	}

	public void StartTask(byte[] wateredNeighbours, byte[] clusterSaturations, float[] moistureLevels, bool[] moistureLevelsChangedLastTick, float[] lastTickMoistureLevels, float[] evaporationModifiers, in ReadOnlyArray<bool> fullMoistureBarriers, in ReadOnlyArray<bool> aboveMoistureBarriers, in ReadOnlyArray<byte> waterColumnCounts, in ReadOnlyArray<ReadOnlyWaterColumn> waterColumns, in ReadOnlyArray<byte> terrainColumnCounts, in ReadOnlyArray<ReadOnlyTerrainColumn> terrainColumns)
	{
		MoistureDataPreparationTask task = new MoistureDataPreparationTask(moistureLevels, lastTickMoistureLevels, moistureLevelsChangedLastTick);
		WateredNeighborsCountingTask task2 = new WateredNeighborsCountingTask(wateredNeighbours, in waterColumnCounts, in waterColumns, _stride, _verticalStride, _mapSize.x);
		ClusterSaturationCalculationTask task3 = new ClusterSaturationCalculationTask(clusterSaturations, new ReadOnlyArray<byte>(wateredNeighbours), in waterColumnCounts, in waterColumns, _maxClusterSaturation, _mapSize.x, _stride, _verticalStride);
		WaterEvaporationCalculationTask task4 = new WaterEvaporationCalculationTask(evaporationModifiers, in waterColumnCounts, new ReadOnlyArray<byte>(clusterSaturations), new ReadOnlyArray<float>(_saturationToEvaporationMap), _mapSize.x, _stride, _verticalStride);
		MoistureCalculationTask task5 = new MoistureCalculationTask(_waterColumnRetriever, _ceilingRetriever, moistureLevels, moistureLevelsChangedLastTick, in waterColumnCounts, in waterColumns, in terrainColumnCounts, in terrainColumns, new ReadOnlyArray<float>(lastTickMoistureLevels), in fullMoistureBarriers, in aboveMoistureBarriers, new ReadOnlyArray<byte>(clusterSaturations), _mapSize.x, _stride, _verticalStride, _moistureDecayRate, _moistureSpreadingRate, _minimumWaterContamination, _verticalSpreadCostMultiplier, _waterContaminationScaler);
		_parallelizer.Schedule(in task).ContinueWith(0, _mapSize.y, 5, in task2).ContinueWith(0, _mapSize.y, 5, in task3)
			.ContinueWith(0, _mapSize.y, 10, in task4)
			.ContinueWith(0, _mapSize.y, 1, in task5);
	}

	private void InitializeEvaporationModifiers(SoilMoistureSimulatorSpec spec)
	{
		int maxEvaporationSaturation = spec.MaxEvaporationSaturation;
		_saturationToEvaporationMap = new float[maxEvaporationSaturation];
		for (int i = 0; i < maxEvaporationSaturation; i++)
		{
			int num = maxEvaporationSaturation - i;
			float num2 = spec.QuadraticEvaporationCoefficient * (float)num * (float)num + spec.LinearQuadraticCoefficient * (float)num + spec.ConstantQuadraticCoefficient;
			_saturationToEvaporationMap[i] = num2;
		}
	}
}
