using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;
using Timberborn.WorkSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Workshops;

public class WorkshopProductivityCounter : TickableComponent, IAwakableComponent, IPostLoadableEntity, IFinishedStateListener, IPersistentEntity
{
	private static readonly ComponentKey WorkshopProductivityCounterKey = new ComponentKey("WorkshopProductivityCounter");

	private static readonly PropertyKey<DailyProductivity> DailyProductivityKey = new PropertyKey<DailyProductivity>("DailyProductivity");

	private readonly IDayNightCycle _dayNightCycle;

	private readonly DailyProductivitySerializer _dailyProductivitySerializer;

	private DailyProductivity _dailyProductivity;

	private Workplace _workplace;

	private Workshop _workshop;

	private WorkplaceWorkingHours _workplaceWorkingHours;

	public WorkshopProductivityCounter(IDayNightCycle dayNightCycle, DailyProductivitySerializer dailyProductivitySerializer)
	{
		_dayNightCycle = dayNightCycle;
		_dailyProductivitySerializer = dailyProductivitySerializer;
	}

	public void Awake()
	{
		_workplace = GetComponent<Workplace>();
		_workshop = GetComponent<Workshop>();
		_workplaceWorkingHours = GetComponent<WorkplaceWorkingHours>();
		_dailyProductivity = DailyProductivity.CreateDefault();
		DisableComponent();
	}

	public void PostLoadEntity()
	{
		if (base.Enabled)
		{
			UpdateCurrentHour();
		}
	}

	public override void Tick()
	{
		CheckAndUpdateCurrentHour();
		CollectSample();
	}

	public float CalculateProductivity()
	{
		return _dailyProductivity.CalculateProductivity();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		UpdateCurrentHour();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(WorkshopProductivityCounterKey).Set(DailyProductivityKey, _dailyProductivity, _dailyProductivitySerializer);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(WorkshopProductivityCounterKey);
		_dailyProductivity = component.Get(DailyProductivityKey, _dailyProductivitySerializer);
	}

	private void UpdateCurrentHour()
	{
		_dailyProductivity.SetCurrentHour((int)_dayNightCycle.HoursPassedToday);
	}

	private void CheckAndUpdateCurrentHour()
	{
		int num = (int)_dayNightCycle.HoursPassedToday;
		if (num != _dailyProductivity.CurrentHour)
		{
			_dailyProductivity.UpdateAndSetCurrentHour(num);
		}
	}

	private void CollectSample()
	{
		if (_workplaceWorkingHours.AreWorkingHours || _workshop.NumberOfWorkersWorking > 0)
		{
			_dailyProductivity.AddSample(_workplace.NumberOfAssignedWorkers, _workshop.NumberOfWorkersWorking);
		}
	}
}
