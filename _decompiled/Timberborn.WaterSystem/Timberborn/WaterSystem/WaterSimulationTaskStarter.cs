using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.MapIndexSystem;
using Timberborn.Multithreading;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.WaterSystem;

internal class WaterSimulationTaskStarter : ILoadableSingleton
{
	private readonly struct SimulationParameters
	{
		public List<DirectedFlow>[] DirectedFlows { get; }

		public WaterFlow[] BaseLevelFlows { get; }

		public WaterColumn[] WaterColumns { get; }

		public ColumnOutflows[] Outflows { get; }

		public float[] ContaminationsBuffer { get; }

		public Diffusions[] BaseLevelDiffusions { get; }

		public byte[] TargetedDiffusionCount { get; }

		public List<TargetedDiffusion>[] TargetedDiffusions { get; }

		public ReadOnlyArray<byte> WaterColumnCounts { get; }

		public ReadOnlyArray<int> LimitedDirections { get; }

		public ReadOnlyArray<float> HeightLimits { get; }

		public ReadOnlyArray<float> InflowLimits { get; }

		public ReadOnlyArray<float> EvaporationModifiers { get; }

		public ReadOnlyList<ThreadSafeWaterSource> WaterSources { get; }

		public ReadOnlyList<WaterChange> WaterChanges { get; }

		public Dictionary<Vector3Int, WaterAmountChange> RemovedWater { get; }

		public SimulationParameters(List<DirectedFlow>[] directedFlows, WaterFlow[] baseLevelFlows, WaterColumn[] waterColumns, ColumnOutflows[] outflows, float[] contaminationsBuffer, Diffusions[] baseLevelDiffusions, byte[] targetedDiffusionCount, List<TargetedDiffusion>[] targetedDiffusions, Dictionary<Vector3Int, WaterAmountChange> removedWater, ReadOnlyArray<byte> waterColumnCounts, ReadOnlyArray<int> limitedDirections, ReadOnlyArray<float> heightLimits, ReadOnlyArray<float> inflowLimits, ReadOnlyArray<float> evaporationModifiers, ReadOnlyList<ThreadSafeWaterSource> waterSources, ReadOnlyList<WaterChange> waterChanges)
		{
			DirectedFlows = directedFlows;
			BaseLevelFlows = baseLevelFlows;
			WaterColumns = waterColumns;
			Outflows = outflows;
			ContaminationsBuffer = contaminationsBuffer;
			BaseLevelDiffusions = baseLevelDiffusions;
			TargetedDiffusionCount = targetedDiffusionCount;
			TargetedDiffusions = targetedDiffusions;
			WaterColumnCounts = waterColumnCounts;
			LimitedDirections = limitedDirections;
			HeightLimits = heightLimits;
			InflowLimits = inflowLimits;
			EvaporationModifiers = evaporationModifiers;
			WaterSources = waterSources;
			WaterChanges = waterChanges;
			RemovedWater = removedWater;
		}
	}

	private static readonly int SubstepCount = 2;

	private readonly IParallelizer _parallelizer;

	private readonly MapIndexService _mapIndexService;

	private readonly ISpecService _specService;

	private readonly FlowLimitCalculator _flowLimitCalculator;

	private readonly WaterFlowRetriever _waterFlowRetriever;

	private readonly WaterDepthSetter _waterDepthSetter;

	private readonly MutableWaterColumnRetriever _mutableWaterColumnRetriever;

	private readonly ITickService _tickService;

	private ImmutableArray<int> _neighborOffsets;

	private Vector3Int _mapSize;

	private int _stride;

	private int _verticalStride;

	private float _deltaTime;

	private float _overflowPressureFactor;

	private float _maxHardDamDecrease;

	private float _hardDamSmoothingFactor;

	private float _minHardDamSmoothing;

	private float _maxHardDamSmoothing;

	private float _hardDamOffset;

	private float _softDamOffset;

	private float _waterSpillThreshold;

	private float _waterFlowFactor;

	private float _outflowBalancingScaler;

	private float _fastEvaporationDepthThreshold;

	private float _fastEvaporationSpeed;

	private float _normalEvaporationSpeed;

	private float _maxWaterContamination;

	private double _diffusionOutflowLimit;

	private double _diffusionDepthLimit;

	private float _diffusionRate;

	public WaterSimulationTaskStarter(IParallelizer parallelizer, MapIndexService mapIndexService, ISpecService specService, FlowLimitCalculator flowLimitCalculator, WaterFlowRetriever waterFlowRetriever, WaterDepthSetter waterDepthSetter, MutableWaterColumnRetriever mutableWaterColumnRetriever, ITickService tickService)
	{
		_parallelizer = parallelizer;
		_mapIndexService = mapIndexService;
		_specService = specService;
		_flowLimitCalculator = flowLimitCalculator;
		_waterFlowRetriever = waterFlowRetriever;
		_waterDepthSetter = waterDepthSetter;
		_mutableWaterColumnRetriever = mutableWaterColumnRetriever;
		_tickService = tickService;
	}

	public void Load()
	{
		_mapSize = _mapIndexService.TotalSize;
		_stride = _mapIndexService.Stride;
		_verticalStride = _mapIndexService.VerticalStride;
		_neighborOffsets = ((IEnumerable<int>)new int[4]
		{
			-_stride,
			-1,
			_stride,
			1
		}).ToImmutableArray();
		WaterSimulatorSpec singleSpec = _specService.GetSingleSpec<WaterSimulatorSpec>();
		_deltaTime = _tickService.TickIntervalInSeconds / (float)SubstepCount;
		_overflowPressureFactor = singleSpec.OverflowPressureFactor;
		_maxHardDamDecrease = singleSpec.MaxHardDamDecrease;
		_hardDamOffset = singleSpec.HardDamOffset;
		_hardDamSmoothingFactor = singleSpec.HardDamSmoothingFactor;
		_minHardDamSmoothing = singleSpec.MinHardDamSmoothing;
		_maxHardDamSmoothing = singleSpec.MaxHardDamSmoothing;
		_softDamOffset = singleSpec.SoftDamOffset;
		_waterSpillThreshold = singleSpec.WaterSpillThreshold;
		_waterFlowFactor = singleSpec.WaterFlowFactor * _deltaTime;
		_outflowBalancingScaler = singleSpec.OutflowBalancingScaler;
		_fastEvaporationDepthThreshold = singleSpec.FastEvaporationDepthThreshold;
		_fastEvaporationSpeed = singleSpec.FastEvaporationSpeed;
		_normalEvaporationSpeed = singleSpec.NormalEvaporationSpeed;
		_maxWaterContamination = singleSpec.MaxWaterContamination;
		_diffusionOutflowLimit = singleSpec.DiffusionOutflowLimit;
		_diffusionDepthLimit = singleSpec.DiffusionDepthLimit;
		_diffusionRate = singleSpec.DiffusionRate;
	}

	public void Simulate(List<DirectedFlow>[] directedFlows, WaterFlow[] baseLevelFlows, WaterColumn[] waterColumns, ColumnOutflows[] outflows, float[] contaminationsBuffer, Diffusions[] baseLevelDiffusions, byte[] targetedDiffusionCount, List<TargetedDiffusion>[] targetedDiffusions, Dictionary<Vector3Int, WaterAmountChange> removedWater, ReadOnlyArray<byte> waterColumnCounts, ReadOnlyArray<int> limitedDirections, ReadOnlyArray<float> heightLimits, ReadOnlyArray<float> inflowLimits, ReadOnlyArray<float> evaporationModifiers, ReadOnlyList<ThreadSafeWaterSource> waterSources, ReadOnlyList<WaterChange> waterChanges)
	{
		SimulationParameters parameters = new SimulationParameters(directedFlows, baseLevelFlows, waterColumns, outflows, contaminationsBuffer, baseLevelDiffusions, targetedDiffusionCount, targetedDiffusions, removedWater, waterColumnCounts, limitedDirections, heightLimits, inflowLimits, evaporationModifiers, waterSources, waterChanges);
		ParallelizerHandle dependency = RunSimulationSubstep(in parameters);
		ParallelizerHandle dependency2 = RunSimulationSubstep(dependency, in parameters);
		ScheduleUpdateWaterChanges(dependency2, in parameters);
	}

	private ParallelizerHandle RunSimulationSubstep(in SimulationParameters parameters)
	{
		ParallelizerHandle dependency = _parallelizer.Schedule<ClearBuffersTask>(CreateClearBuffersTask(in parameters));
		return ScheduleSimulationSteps(dependency, in parameters);
	}

	private ParallelizerHandle RunSimulationSubstep(ParallelizerHandle dependency, in SimulationParameters parameters)
	{
		ParallelizerHandle dependency2 = _parallelizer.Schedule<ClearBuffersTask>(CreateClearBuffersTask(in parameters), dependency);
		return ScheduleSimulationSteps(dependency2, in parameters);
	}

	private static ClearBuffersTask CreateClearBuffersTask(in SimulationParameters parameters)
	{
		return new ClearBuffersTask(parameters.ContaminationsBuffer, parameters.TargetedDiffusionCount, parameters.BaseLevelFlows, parameters.BaseLevelDiffusions);
	}

	private ParallelizerHandle ScheduleSimulationSteps(ParallelizerHandle dependency, in SimulationParameters parameters)
	{
		OutflowsUpdateTask task = new OutflowsUpdateTask(_flowLimitCalculator, _waterFlowRetriever, parameters.DirectedFlows, parameters.BaseLevelFlows, parameters.WaterColumnCounts, new ReadOnlyArray<WaterColumn>(parameters.WaterColumns), parameters.LimitedDirections, parameters.HeightLimits, parameters.InflowLimits, new ReadOnlyArray<ColumnOutflows>(parameters.Outflows), _mapSize.x, _stride, _verticalStride, _deltaTime, _overflowPressureFactor, _maxHardDamDecrease, _hardDamSmoothingFactor, _minHardDamSmoothing, _maxHardDamSmoothing, _hardDamOffset, _softDamOffset, _waterSpillThreshold, _waterFlowFactor);
		WaterParametersUpdateTask task2 = new WaterParametersUpdateTask(_waterDepthSetter, parameters.WaterColumns, parameters.Outflows, parameters.WaterColumnCounts, _neighborOffsets, new ReadOnlyArray<WaterFlow>(parameters.BaseLevelFlows), parameters.EvaporationModifiers, new ReadOnlyArray<List<DirectedFlow>>(parameters.DirectedFlows), _mapSize.x, _stride, _verticalStride, _deltaTime, _outflowBalancingScaler, _fastEvaporationDepthThreshold, _fastEvaporationSpeed, _normalEvaporationSpeed);
		SimulateContaminationTask task3 = new SimulateContaminationTask(_flowLimitCalculator, _waterFlowRetriever, parameters.ContaminationsBuffer, parameters.BaseLevelDiffusions, parameters.TargetedDiffusionCount, parameters.TargetedDiffusions, parameters.WaterColumnCounts, new ReadOnlyArray<WaterColumn>(parameters.WaterColumns), new ReadOnlyArray<ColumnOutflows>(parameters.Outflows), parameters.LimitedDirections, parameters.HeightLimits, _mapSize.x, _stride, _verticalStride, _deltaTime, _overflowPressureFactor, _maxWaterContamination, _diffusionOutflowLimit, _diffusionDepthLimit);
		UpdateContaminationTask task4 = new UpdateContaminationTask(parameters.WaterColumns, parameters.WaterColumnCounts, new ReadOnlyArray<float>(parameters.ContaminationsBuffer), new ReadOnlyArray<Diffusions>(parameters.BaseLevelDiffusions), new ReadOnlyArray<byte>(parameters.TargetedDiffusionCount), new ReadOnlyArray<List<TargetedDiffusion>>(parameters.TargetedDiffusions), _mapSize.x, _stride, _verticalStride, _deltaTime, _maxWaterContamination, _diffusionRate);
		UpdateWaterSourcesTask task5 = new UpdateWaterSourcesTask(_mapIndexService, _waterDepthSetter, _mutableWaterColumnRetriever, parameters.WaterColumns, parameters.WaterColumnCounts, parameters.WaterSources, _verticalStride, _deltaTime, _overflowPressureFactor, _maxWaterContamination);
		return _parallelizer.Schedule(0, _mapSize.y, 1, in task, dependency).ContinueWith(0, _mapSize.y, 1, in task2).ContinueWith(0, _mapSize.y, 1, in task3)
			.ContinueWith(0, _mapSize.y, 3, in task4)
			.ContinueWith(in task5);
	}

	private void ScheduleUpdateWaterChanges(ParallelizerHandle dependency, in SimulationParameters parameters)
	{
		UpdateWaterChangesTask task = new UpdateWaterChangesTask(_mapIndexService, _waterDepthSetter, _mutableWaterColumnRetriever, parameters.RemovedWater, parameters.WaterColumns, parameters.WaterColumnCounts, parameters.WaterChanges, _verticalStride, _overflowPressureFactor, _maxWaterContamination);
		_parallelizer.Schedule(in task, dependency);
	}
}
