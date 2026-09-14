using Timberborn.AchievementSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;
using Timberborn.WorkSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Achievements;

internal class WorkAllDayForWeekAchievement : Achievement, ILoadableSingleton, ISaveableSingleton
{
	private static readonly SingletonKey WorkAllDayForWeekAchievementKey = new SingletonKey("WorkAllDayForWeekAchievement");

	private static readonly PropertyKey<float> ProgressKey = new PropertyKey<float>("Progress");

	private static readonly int WorkingHoursRequired = 24;

	private static readonly int WorkingDaysRequired = 7;

	private readonly EventBus _eventBus;

	private readonly ISingletonLoader _singletonLoader;

	private readonly WorkingHoursManager _workingHoursManager;

	private readonly ITimeTrigger _timeTrigger;

	public override string Id => "WORK_ALL_DAY_FOR_WEEK";

	public WorkAllDayForWeekAchievement(EventBus eventBus, ISingletonLoader singletonLoader, WorkingHoursManager workingHoursManager, ITimeTriggerFactory timeTriggerFactory)
	{
		_eventBus = eventBus;
		_singletonLoader = singletonLoader;
		_workingHoursManager = workingHoursManager;
		_timeTrigger = timeTriggerFactory.Create(base.Unlock, WorkingDaysRequired);
	}

	[OnEvent]
	public void OnWorkingHoursChanged(WorkingHoursChangedEvent workingHoursChangedEvent)
	{
		CheckTimer();
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (_timeTrigger.Progress != 0f)
		{
			singletonSaver.GetSingleton(WorkAllDayForWeekAchievementKey).Set(ProgressKey, _timeTrigger.Progress);
		}
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(WorkAllDayForWeekAchievementKey, out var objectLoader))
		{
			_timeTrigger.FastForwardProgress(objectLoader.Get(ProgressKey));
		}
	}

	protected override void EnableInternal()
	{
		_eventBus.Register(this);
		CheckTimer();
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
		_timeTrigger.Reset();
	}

	private void CheckTimer()
	{
		if (_workingHoursManager.EndHours >= (float)WorkingHoursRequired)
		{
			_timeTrigger.Resume();
		}
		else
		{
			_timeTrigger.Reset();
		}
	}
}
