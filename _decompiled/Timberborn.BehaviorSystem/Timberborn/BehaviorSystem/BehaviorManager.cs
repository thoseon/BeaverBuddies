using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Metrics;
using Timberborn.Persistence;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.BehaviorSystem;

public class BehaviorManager : TickableComponent, IAwakableComponent, IPersistentEntity, ILateTickable
{
	private static readonly ComponentKey BehaviorManagerKey = new ComponentKey("BehaviorManager");

	private static readonly PropertyKey<Behavior> RunningBehaviorKey = new PropertyKey<Behavior>("RunningBehavior");

	private static readonly PropertyKey<bool> ReturnToBehaviorKey = new PropertyKey<bool>("ReturnToBehavior");

	private static readonly PropertyKey<string> RunningExecutorIdKey = new PropertyKey<string>("RunningExecutorId");

	private static readonly PropertyKey<float> RunningExecutorElapsedTimeKey = new PropertyKey<float>("RunningExecutorElapsedTime");

	private static readonly ListKey<string> TimestampedBehaviorLogKey = new ListKey<string>("TimestampedBehaviorLog");

	private readonly IDayNightCycle _dayNightCycle;

	private readonly TimerMetricCache<RootBehavior> _timerMetricCache;

	private readonly IMetricsService _metricsService;

	private readonly ReferenceSerializer _referenceSerializer;

	private readonly ITickService _tickService;

	private BehaviorAgent _behaviorAgent;

	private readonly List<RootBehavior> _rootBehaviors = new List<RootBehavior>();

	private readonly CyclicBuffer<string> _timestampedBehaviorLog = new CyclicBuffer<string>(10);

	private Behavior _runningBehavior;

	private bool _returnToBehavior;

	private IExecutor _runningExecutor;

	private float _runningExecutorElapsedTime;

	public BehaviorInfo RunningBehavior => new BehaviorInfo(_runningBehavior?.ComponentName);

	public ExecutorInfo RunningExecutor
	{
		get
		{
			if (_runningExecutor == null)
			{
				return default(ExecutorInfo);
			}
			return new ExecutorInfo(_runningExecutor.GetName(), _runningExecutorElapsedTime);
		}
	}

	public IEnumerable<string> TimestampedBehaviorLog => _timestampedBehaviorLog.Values.AsReadOnlyEnumerable();

	public BehaviorManager(IDayNightCycle dayNightCycle, TimerMetricCache<RootBehavior> timerMetricCache, IMetricsService metricsService, ReferenceSerializer referenceSerializer, ITickService tickService)
	{
		_dayNightCycle = dayNightCycle;
		_timerMetricCache = timerMetricCache;
		_metricsService = metricsService;
		_referenceSerializer = referenceSerializer;
		_tickService = tickService;
	}

	public void Awake()
	{
		_behaviorAgent = GetComponent<BehaviorAgent>();
	}

	public override void Tick()
	{
		if (_runningExecutor != null)
		{
			TickRunningExecutor();
		}
		if (_runningExecutor == null)
		{
			ProcessBehaviors();
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(BehaviorManagerKey);
		SaveRunningBehavior(component);
		SaveRunningExecutor(entitySaver, component);
		component.Set(TimestampedBehaviorLogKey, _timestampedBehaviorLog.Values.ToList());
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(BehaviorManagerKey);
		LoadRunningBehavior(component);
		LoadRunningExecutor(entityLoader, component);
		_timestampedBehaviorLog.AddRange(component.Get(TimestampedBehaviorLogKey));
	}

	public void AddRootBehavior(RootBehavior rootBehavior)
	{
		_rootBehaviors.Add(rootBehavior);
	}

	public bool IsRunningBehavior<TBehavior>()
	{
		return _runningBehavior is TBehavior;
	}

	public bool IsRunningExecutor<TExecutor>() where TExecutor : IExecutor
	{
		return _runningExecutor is TExecutor;
	}

	private void SaveRunningBehavior(IObjectSaver behaviorManager)
	{
		if ((bool)_runningBehavior)
		{
			behaviorManager.Set(RunningBehaviorKey, _runningBehavior, _referenceSerializer.Of<Behavior>());
			behaviorManager.Set(ReturnToBehaviorKey, _returnToBehavior);
		}
	}

	private void LoadRunningBehavior(IObjectLoader behaviorManager)
	{
		if (behaviorManager.Has(RunningBehaviorKey) && behaviorManager.GetObsoletable(RunningBehaviorKey, _referenceSerializer.Of<Behavior>(), out _runningBehavior))
		{
			_returnToBehavior = behaviorManager.Get(ReturnToBehaviorKey);
		}
	}

	private void SaveRunningExecutor(IEntitySaver entitySaver, IObjectSaver behaviorManager)
	{
		if (_runningExecutor != null)
		{
			behaviorManager.Set(RunningExecutorIdKey, SerializeExecutor(_runningExecutor));
			behaviorManager.Set(RunningExecutorElapsedTimeKey, _runningExecutorElapsedTime);
			_runningExecutor.Save(entitySaver);
		}
	}

	private void LoadRunningExecutor(IEntityLoader entityLoader, IObjectLoader behaviorManager)
	{
		if (behaviorManager.Has(RunningExecutorIdKey) && _runningBehavior != null)
		{
			_runningExecutor = DeserializeExecutor(behaviorManager.Get(RunningExecutorIdKey));
			if (_runningExecutor != null)
			{
				_runningExecutor.Load(entityLoader);
				_runningExecutorElapsedTime = behaviorManager.Get(RunningExecutorElapsedTimeKey);
			}
		}
	}

	private void ProcessBehaviors()
	{
		if (_returnToBehavior && (bool)_runningBehavior && ProcessBehavior(_runningBehavior))
		{
			return;
		}
		foreach (RootBehavior rootBehavior in _rootBehaviors)
		{
			if (_metricsService.MetricsEnabled)
			{
				_timerMetricCache.Get(rootBehavior).Resume();
			}
			bool num = ProcessBehavior(rootBehavior);
			if (_metricsService.MetricsEnabled)
			{
				_timerMetricCache.Get(rootBehavior).Pause();
			}
			if (num)
			{
				break;
			}
		}
	}

	private bool ProcessBehavior(Behavior behavior)
	{
		Decision decision = behavior.Decide(_behaviorAgent);
		if (!decision.ShouldReleaseNow)
		{
			if (decision.Executor != null)
			{
				_runningExecutor = decision.Executor;
				_runningExecutorElapsedTime = 0f;
			}
			SetRunningBehavior(decision.Behavior ?? behavior);
			_returnToBehavior = decision.ShouldReturnToBehavior;
			return true;
		}
		_runningBehavior = null;
		return false;
	}

	private void SetRunningBehavior(Behavior behavior)
	{
		if (_runningBehavior != behavior)
		{
			string value = $"{behavior.ComponentName} {_dayNightCycle.PartialDayNumber:0.00}";
			_timestampedBehaviorLog.Add(value);
		}
		_runningBehavior = behavior;
	}

	private void TickRunningExecutor()
	{
		_runningExecutorElapsedTime += _tickService.TickIntervalInSeconds;
		switch (_runningExecutor.Tick(_dayNightCycle.FixedDeltaTimeInHours))
		{
		case ExecutorStatus.Success:
		case ExecutorStatus.Failure:
			_runningExecutor = null;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case ExecutorStatus.Running:
			break;
		}
	}

	private static string SerializeExecutor(IExecutor executor)
	{
		return executor.GetName();
	}

	private IExecutor DeserializeExecutor(string id)
	{
		return GetComponentsAllocating<IExecutor>().SingleOrDefault((IExecutor executor) => executor.GetName() == id);
	}
}
