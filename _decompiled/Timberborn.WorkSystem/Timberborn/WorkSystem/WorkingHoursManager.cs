using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.WorkSystem;

public class WorkingHoursManager : ISaveableSingleton, ILoadableSingleton, ITickableSingleton
{
	private static readonly SingletonKey WorkingHoursManagerKey = new SingletonKey("WorkingHoursManager");

	private static readonly PropertyKey<float> WorkedPartOfDayKey = new PropertyKey<float>("WorkedPartOfDay");

	private float _startHours;

	private float _workedPartOfDay;

	private bool _wereWorkingHours;

	private readonly ISingletonLoader _singletonLoader;

	private readonly IDayNightCycle _dayNightCycle;

	private readonly EventBus _eventBus;

	public float EndHours { get; private set; }

	public bool AreWorkingHours
	{
		get
		{
			float hoursPassedToday = _dayNightCycle.HoursPassedToday;
			if (_workedPartOfDay > 0f && hoursPassedToday >= _startHours)
			{
				return hoursPassedToday < EndHours;
			}
			return false;
		}
	}

	public float WorkedPartOfDay
	{
		get
		{
			return _workedPartOfDay;
		}
		set
		{
			_workedPartOfDay = value;
			EndHours = _startHours + _workedPartOfDay * 24f;
			_eventBus.Post(new WorkingHoursChangedEvent());
		}
	}

	public WorkingHoursManager(ISingletonLoader singletonLoader, IDayNightCycle dayNightCycle, EventBus eventBus)
	{
		_singletonLoader = singletonLoader;
		_dayNightCycle = dayNightCycle;
		_eventBus = eventBus;
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(WorkingHoursManagerKey, out var objectLoader))
		{
			_startHours = _dayNightCycle.BoundsInHours(TimeOfDay.Daytime).start;
			WorkedPartOfDay = objectLoader.Get(WorkedPartOfDayKey);
		}
		else
		{
			(float, float) tuple = _dayNightCycle.BoundsInHours(TimeOfDay.Daytime);
			_startHours = tuple.Item1;
			EndHours = tuple.Item2;
			float num = EndHours - _startHours;
			WorkedPartOfDay = num / 24f;
		}
		_wereWorkingHours = AreWorkingHours;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		singletonSaver.GetSingleton(WorkingHoursManagerKey).Set(WorkedPartOfDayKey, _workedPartOfDay);
	}

	public void Tick()
	{
		bool areWorkingHours = AreWorkingHours;
		if (_wereWorkingHours != areWorkingHours)
		{
			_eventBus.Post(new WorkingHoursTransitionedEvent());
		}
		_wereWorkingHours = areWorkingHours;
	}
}
