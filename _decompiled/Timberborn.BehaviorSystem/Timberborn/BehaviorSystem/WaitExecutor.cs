using Timberborn.BaseComponentSystem;
using Timberborn.Persistence;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.BehaviorSystem;

public class WaitExecutor : BaseComponent, IExecutor
{
	private static readonly ComponentKey WaitExecutorKey = new ComponentKey("WaitExecutor");

	private static readonly PropertyKey<float> FinishTimestampKey = new PropertyKey<float>("FinishTimestamp");

	private static readonly float IdleWaitTimeInHours = 0.15f;

	private readonly IDayNightCycle _dayNightCycle;

	private float _finishTimestamp;

	public WaitExecutor(IDayNightCycle dayNightCycle)
	{
		_dayNightCycle = dayNightCycle;
	}

	public void LaunchForIdleTime()
	{
		LaunchForSpecifiedTime(IdleWaitTimeInHours);
	}

	public void LaunchForSpecifiedTime(float hours)
	{
		float finishTimestamp = _dayNightCycle.DayNumberHoursFromNow(hours);
		_finishTimestamp = finishTimestamp;
	}

	public ExecutorStatus Tick(float deltaTimeInHours)
	{
		if (_dayNightCycle.PartialDayNumber > _finishTimestamp)
		{
			return ExecutorStatus.Success;
		}
		return ExecutorStatus.Running;
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(WaitExecutorKey).Set(FinishTimestampKey, _finishTimestamp);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(WaitExecutorKey);
		_finishTimestamp = component.Get(FinishTimestampKey);
	}
}
