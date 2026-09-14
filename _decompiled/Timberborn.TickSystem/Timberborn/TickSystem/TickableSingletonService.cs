using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Timberborn.Metrics;
using Timberborn.Multithreading;
using Timberborn.SingletonSystem;

namespace Timberborn.TickSystem;

internal class TickableSingletonService : ILoadableSingleton, ITickableSingletonService
{
	private readonly struct MeteredSingleton(ITickableSingleton tickableSingleton, ITimerMetric metric, bool metricsEnabled)
	{
		private readonly ITickableSingleton _tickableSingleton = tickableSingleton;

		private readonly ITimerMetric _metric = metric;

		private readonly bool _metricsEnabled = metricsEnabled;

		public void Tick()
		{
			if (_metricsEnabled)
			{
				_metric.Resume();
			}
			_tickableSingleton.Tick();
			if (_metricsEnabled)
			{
				_metric.Pause();
			}
		}
	}

	private static readonly string WaitStartMarkerId = "Wait - Start";

	private static readonly string WaitEndMarkerId = "Wait - End";

	private static readonly string ScheduleStartMarkerId = "Schedule - Start";

	private static readonly string ScheduleEndMarkerId = "Schedule - End";

	private readonly ISingletonRepository _singletonRepository;

	private readonly ITickingMode _tickingMode;

	private readonly IMetricsService _metricsService;

	private readonly IParallelizer _parallelizer;

	private readonly ISnapshotCollector _snapshotCollector;

	private ImmutableArray<MeteredSingleton> _tickableSingletons;

	private ImmutableArray<IParallelTickableSingleton> _parallelTickableSingletons;

	private long _parallelTickStartTimestamp;

	public TimeSpan LastParallelTickDuration { get; private set; }

	public bool ParalleTicklIsFinished { get; private set; }

	public bool IsStartingParallelTick { get; private set; }

	public event EventHandler ForcedParallelTickFinished;

	public TickableSingletonService(ISingletonRepository singletonRepository, ITickingMode tickingMode, IMetricsService metricsService, IParallelizer parallelizer, ISnapshotCollector snapshotCollector)
	{
		_singletonRepository = singletonRepository;
		_tickingMode = tickingMode;
		_metricsService = metricsService;
		_parallelizer = parallelizer;
		_snapshotCollector = snapshotCollector;
	}

	public void Load()
	{
		_tickableSingletons = (from tickable in _singletonRepository.GetSingletons<ITickableSingleton>().Where(_tickingMode.SingletonIsActiveInThisMode)
			orderby (tickable is ILateTickable) ? 1 : 0
			select tickable).Select(CreateMeteredSingleton).ToImmutableArray();
		_parallelTickableSingletons = _singletonRepository.GetSingletons<IParallelTickableSingleton>().Where(_tickingMode.SingletonIsActiveInThisMode).ToImmutableArray();
	}

	public void TickAll()
	{
		FinishParallelTick();
		TickSingletons();
		StartParallelTick();
	}

	public void ForceFinishParallelTick()
	{
		FinishParallelTick();
		ForcedParallelTickFinished?.Invoke(this, EventArgs.Empty);
	}

	private void StartParallelTick()
	{
		ParalleTicklIsFinished = false;
		IsStartingParallelTick = true;
		_parallelizer.StartScheduling();
		_parallelTickStartTimestamp = Stopwatch.GetTimestamp();
		_snapshotCollector.AddMarker(ScheduleStartMarkerId);
		foreach (IParallelTickableSingleton parallelTickableSingleton in _parallelTickableSingletons)
		{
			parallelTickableSingleton.StartParallelTick();
		}
		_snapshotCollector.AddMarker(ScheduleEndMarkerId);
		_parallelizer.StopScheduling();
		IsStartingParallelTick = false;
	}

	private void TickSingletons()
	{
		for (int i = 0; i < _tickableSingletons.Length; i++)
		{
			_tickableSingletons[i].Tick();
		}
	}

	private void FinishParallelTick()
	{
		_snapshotCollector.AddMarker(WaitStartMarkerId);
		_parallelizer.Wait();
		_snapshotCollector.AddMarker(WaitEndMarkerId);
		LastParallelTickDuration = TimeSpan.FromTicks(_parallelizer.LastTaskTimestamp - _parallelTickStartTimestamp);
		_parallelizer.ThrowIfAnyPendingTasks();
		ParalleTicklIsFinished = true;
	}

	private MeteredSingleton CreateMeteredSingleton(ITickableSingleton tickableSingleton)
	{
		string name = tickableSingleton.GetType().Name;
		ITimerMetric timerMetric = _metricsService.GetTimerMetric("Tick", name);
		return new MeteredSingleton(tickableSingleton, timerMetric, _metricsService.MetricsEnabled);
	}
}
