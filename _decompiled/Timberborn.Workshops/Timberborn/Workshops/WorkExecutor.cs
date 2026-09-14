using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.BlockingSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.TimeSystem;
using Timberborn.WorkSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Workshops;

public class WorkExecutor : BaseComponent, IAwakableComponent, IInitializableEntity, IExecutor
{
	private static readonly ComponentKey WorkExecutorKey = new ComponentKey("WorkExecutor");

	private static readonly PropertyKey<float> FinishTimestampKey = new PropertyKey<float>("FinishTimestamp");

	private readonly IDayNightCycle _dayNightCycle;

	private Worker _worker;

	private Workshop _workshop;

	private BlockableObject _blockableObject;

	private float _finishTimestamp;

	private bool _isWorking;

	public WorkExecutor(IDayNightCycle dayNightCycle)
	{
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		_worker = GetComponent<Worker>();
		_worker.GotUnemployed += OnGotUnemployed;
	}

	public void InitializeEntity()
	{
		if ((bool)_workshop)
		{
			StartWorking();
		}
	}

	public void Launch(float maxWorkingTimeInHours)
	{
		Initialize();
		if ((bool)_workshop && _blockableObject.IsUnblocked)
		{
			_finishTimestamp = _dayNightCycle.DayNumberHoursFromNow(maxWorkingTimeInHours);
			StartWorking();
		}
	}

	public ExecutorStatus Tick(float deltaTimeInHours)
	{
		if (!_workshop || !_isWorking)
		{
			return ExecutorStatus.Failure;
		}
		if (!_blockableObject.IsUnblocked)
		{
			StopWorking();
			return ExecutorStatus.Failure;
		}
		if (_dayNightCycle.PartialDayNumber > _finishTimestamp)
		{
			StopWorking();
			return ExecutorStatus.Success;
		}
		return ExecutorStatus.Running;
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(WorkExecutorKey).Set(FinishTimestampKey, _finishTimestamp);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(WorkExecutorKey);
		_finishTimestamp = component.Get(FinishTimestampKey);
		Initialize();
	}

	private void Initialize()
	{
		Workplace workplace = _worker.Workplace;
		if ((bool)workplace)
		{
			_workshop = workplace.GetComponent<Workshop>();
			_blockableObject = workplace.GetComponent<BlockableObject>();
		}
		else
		{
			Clear();
		}
	}

	private void StartWorking()
	{
		_workshop.InformOfStartedWorking();
		_isWorking = true;
	}

	private void StopWorking()
	{
		_workshop.InformOfStoppedWorking();
		_isWorking = false;
	}

	private void OnGotUnemployed(object sender, EventArgs e)
	{
		if ((bool)_workshop && _isWorking)
		{
			StopWorking();
			Clear();
		}
	}

	private void Clear()
	{
		_workshop = null;
		_blockableObject = null;
	}
}
