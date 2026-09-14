using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Metrics;

namespace Timberborn.WorkSystem;

public class WorkerRootBehavior : RootBehavior, IAwakableComponent
{
	private readonly TimerMetricCache<WorkplaceBehavior> _timerMetricCache;

	private readonly IMetricsService _metricsService;

	private Worker _worker;

	private WorkerWorkingHours _workerWorkingHours;

	private WorkRefuser _workRefuser;

	private BehaviorAgent _behaviorAgent;

	private CommunityServiceBehavior _communityServiceBehavior;

	public WorkerRootBehavior(TimerMetricCache<WorkplaceBehavior> timerMetricCache, IMetricsService metricsService)
	{
		_timerMetricCache = timerMetricCache;
		_metricsService = metricsService;
	}

	public void Awake()
	{
		_worker = GetComponent<Worker>();
		_workerWorkingHours = GetComponent<WorkerWorkingHours>();
		_workRefuser = GetComponent<WorkRefuser>();
		_behaviorAgent = GetComponent<BehaviorAgent>();
		_communityServiceBehavior = GetComponent<CommunityServiceBehavior>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (!_workRefuser.RefusesWork)
		{
			if (_worker.Employed && _workerWorkingHours.AreWorkingHours)
			{
				return DecideAsWorker();
			}
			return _communityServiceBehavior.Decide(agent);
		}
		return Decision.ReleaseNow();
	}

	private Decision DecideAsWorker()
	{
		if (_worker.Workplace.Overstaffed)
		{
			_worker.Unemploy();
			return Decision.ReleaseNextTick();
		}
		return WorkAtWorkplace();
	}

	private Decision WorkAtWorkplace()
	{
		foreach (WorkplaceBehavior workplaceBehavior in _worker.Workplace.WorkplaceBehaviors)
		{
			if (_metricsService.MetricsEnabled)
			{
				_timerMetricCache.Get(workplaceBehavior).Resume();
			}
			Decision decision = workplaceBehavior.Decide(_behaviorAgent);
			if (_metricsService.MetricsEnabled)
			{
				_timerMetricCache.Get(workplaceBehavior).Pause();
			}
			if (!decision.ShouldReleaseNow)
			{
				return Decision.TransferNow(workplaceBehavior, in decision);
			}
		}
		return Decision.ReleaseNow();
	}
}
