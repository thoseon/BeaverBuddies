using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.BlockingSystem;
using Timberborn.EntitySystem;
using Timberborn.MechanicalSystem;
using Timberborn.Persistence;
using Timberborn.TimeSystem;
using Timberborn.WorkSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Workshops;

public class ProduceExecutor : BaseComponent, IAwakableComponent, IInitializableEntity, IExecutor
{
	private static readonly ComponentKey ProduceExecutorKey = new ComponentKey("ProduceExecutor");

	private static readonly PropertyKey<float> FinishTimestampKey = new PropertyKey<float>("FinishTimestamp");

	private static readonly PropertyKey<Workplace> WorkplaceKey = new PropertyKey<Workplace>("Workplace");

	private readonly IDayNightCycle _dayNightCycle;

	private readonly ReferenceSerializer _referenceSerializer;

	private Worker _worker;

	private Workshop _workshop;

	private MechanicalBuilding _mechanicalBuilding;

	private Workplace _workplace;

	private Manufactory _manufactory;

	private BlockableObject _blockableObject;

	private float _finishTimestamp;

	private bool _isProducing;

	public ProduceExecutor(IDayNightCycle dayNightCycle, ReferenceSerializer referenceSerializer)
	{
		_dayNightCycle = dayNightCycle;
		_referenceSerializer = referenceSerializer;
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
			StartProducing();
		}
	}

	public bool Launch(float maxProductionTimeInHours)
	{
		Initialize(_worker.Workplace);
		if (!_workplace || !_blockableObject.IsUnblocked)
		{
			return false;
		}
		if (!_manufactory || !_manufactory.IsReadyToProduce)
		{
			return false;
		}
		if ((bool)_mechanicalBuilding && !_mechanicalBuilding.ActiveAndPowered)
		{
			return false;
		}
		_finishTimestamp = _dayNightCycle.DayNumberHoursFromNow(maxProductionTimeInHours);
		StartProducing();
		return true;
	}

	public ExecutorStatus Tick(float deltaTimeInHours)
	{
		if (!_workplace || !_isProducing)
		{
			return ExecutorStatus.Failure;
		}
		if (!_blockableObject.IsUnblocked)
		{
			StopProducing();
			return ExecutorStatus.Failure;
		}
		if (_dayNightCycle.PartialDayNumber > _finishTimestamp || !_manufactory.IsReadyToProduce)
		{
			StopProducing();
			return ExecutorStatus.Success;
		}
		_manufactory.IncreaseProductionProgress(deltaTimeInHours * _worker.WorkingSpeedMultiplier);
		return ExecutorStatus.Running;
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(ProduceExecutorKey);
		component.Set(FinishTimestampKey, _finishTimestamp);
		if ((bool)_workplace)
		{
			component.Set(WorkplaceKey, _workplace, _referenceSerializer.Of<Workplace>());
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ProduceExecutorKey);
		_finishTimestamp = component.Get(FinishTimestampKey);
		Workplace workplace = (component.Has(WorkplaceKey) ? component.Get(WorkplaceKey, _referenceSerializer.Of<Workplace>()) : _worker.Workplace);
		Initialize(workplace);
		VerifyLoadedWorkplace();
	}

	private void Initialize(Workplace workplace)
	{
		if ((bool)workplace)
		{
			_workplace = workplace;
			_workshop = _workplace.GetComponent<Workshop>();
			_manufactory = _workplace.GetComponent<Manufactory>();
			_blockableObject = _workplace.GetComponent<BlockableObject>();
			_mechanicalBuilding = _workplace.GetComponent<MechanicalBuilding>();
		}
	}

	private void StartProducing()
	{
		_workshop.InformOfStartedWorking();
		_isProducing = true;
	}

	private void StopProducing()
	{
		_workshop.InformOfStoppedWorking();
		_isProducing = false;
	}

	private void VerifyLoadedWorkplace()
	{
		if (!_workplace || !_workshop || !_manufactory)
		{
			Clear();
		}
	}

	private void OnGotUnemployed(object sender, EventArgs e)
	{
		if ((bool)_workshop && _isProducing)
		{
			StopProducing();
			Clear();
		}
	}

	private void Clear()
	{
		_workplace = null;
		_workshop = null;
		_manufactory = null;
		_blockableObject = null;
		_mechanicalBuilding = null;
	}
}
