using System;
using Timberborn.BlueprintSystem;
using Timberborn.MapStateSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.TimeSystem;

internal class DayNightCycle : IDayNightCycle, ISaveableSingleton, ILoadableSingleton, ITickableSingleton
{
	private static readonly SingletonKey DayNightCycleKey = new SingletonKey("DayNightCycle");

	private static readonly PropertyKey<int> DayNumberKey = new PropertyKey<int>("DayNumber");

	private static readonly PropertyKey<float> DayProgressKey = new PropertyKey<float>("DayProgress");

	private readonly EventBus _eventBus;

	private readonly ISingletonLoader _singletonLoader;

	private readonly MapEditorMode _mapEditorMode;

	private readonly ISpecService _specService;

	private readonly ITickService _tickService;

	private readonly ITickProgressService _tickProgressService;

	private DayNightCycleSpec _dayNightCycleSpec;

	private int _daytimeLengthInTicks;

	private int _ticksPassedToday;

	public int DayNumber { get; private set; }

	public float DayLengthInSeconds { get; private set; }

	public float DaytimeLengthInHours { get; private set; }

	public float NighttimeLengthInHours { get; private set; }

	public float FixedDeltaTimeInHours { get; private set; }

	public float DayProgress => (float)_ticksPassedToday * 1f / (float)_dayNightCycleSpec.ConfiguredDayLengthInTicks;

	public float PartialDayNumber => (float)DayNumber + DayProgress;

	public bool IsDaytime => TimeOfDay == TimeOfDay.Daytime;

	public bool IsNighttime => TimeOfDay == TimeOfDay.Nighttime;

	public float HoursPassedToday => TicksToHours(_ticksPassedToday);

	public float FluidSecondsPassedToday => (float)_ticksPassedToday * _tickService.TickIntervalInSeconds + _tickProgressService.SecondsPassedThisTick;

	private TimeOfDay TimeOfDay
	{
		get
		{
			if (_ticksPassedToday >= _daytimeLengthInTicks)
			{
				return TimeOfDay.Nighttime;
			}
			return TimeOfDay.Daytime;
		}
	}

	public DayNightCycle(EventBus eventBus, ISingletonLoader singletonLoader, MapEditorMode mapEditorMode, ISpecService specService, ITickService tickService, ITickProgressService tickProgressService)
	{
		_eventBus = eventBus;
		_singletonLoader = singletonLoader;
		_mapEditorMode = mapEditorMode;
		_specService = specService;
		_tickService = tickService;
		_tickProgressService = tickProgressService;
	}

	public void Load()
	{
		InitializeConfigurationValues();
		if (_singletonLoader.TryGetSingleton(DayNightCycleKey, out var objectLoader))
		{
			DayNumber = objectLoader.Get(DayNumberKey);
			_ticksPassedToday = (int)(objectLoader.Get(DayProgressKey) * (float)_dayNightCycleSpec.ConfiguredDayLengthInTicks);
		}
		else
		{
			_ticksPassedToday = HoursToTicks(_dayNightCycleSpec.HoursPassedOnNewGame);
			DayNumber = 1;
		}
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (!_mapEditorMode.IsMapEditor)
		{
			IObjectSaver singleton = singletonSaver.GetSingleton(DayNightCycleKey);
			singleton.Set(DayNumberKey, DayNumber);
			singleton.Set(DayProgressKey, DayProgress);
		}
	}

	public void Tick()
	{
		ProgressTimeBy(1);
	}

	public (float start, float end) BoundsInHours(TimeOfDay timeOfDay)
	{
		return timeOfDay switch
		{
			TimeOfDay.Daytime => (start: 0f, end: DaytimeLengthInHours), 
			TimeOfDay.Nighttime => (start: DaytimeLengthInHours, end: 24f), 
			_ => throw new ArgumentOutOfRangeException("timeOfDay", timeOfDay, null), 
		};
	}

	public float SecondsToHours(float seconds)
	{
		return seconds / DayLengthInSeconds * 24f;
	}

	public float DayNumberHoursFromNow(float hours)
	{
		return PartialDayNumber + hours / 24f;
	}

	public float HoursToNextStartOf(TimeOfDay timeOfDay)
	{
		return HoursToNextStartOf(timeOfDay, HoursPassedToday);
	}

	public float FluidHoursToNextStartOf(TimeOfDay timeOfDay)
	{
		return HoursToNextStartOf(timeOfDay, SecondsToHours(FluidSecondsPassedToday));
	}

	public float TicksToHours(int ticks)
	{
		return (float)ticks / (float)_dayNightCycleSpec.ConfiguredDayLengthInTicks * 24f;
	}

	public int HoursToTicks(float hours)
	{
		return (int)(hours / 24f * (float)_dayNightCycleSpec.ConfiguredDayLengthInTicks);
	}

	public void SetTimeToNextDay()
	{
		if (IsDaytime)
		{
			ProgressTimeBy(_daytimeLengthInTicks - _ticksPassedToday);
		}
		ProgressTimeBy(_dayNightCycleSpec.ConfiguredDayLengthInTicks - _ticksPassedToday);
	}

	public void JumpTimeInHours(float hours)
	{
		ProgressTimeBy(HoursToTicks(hours));
	}

	private float HoursToNextStartOf(TimeOfDay timeOfDay, float hoursPassedToday)
	{
		return timeOfDay switch
		{
			TimeOfDay.Daytime => HoursToNextHour(0f, hoursPassedToday), 
			TimeOfDay.Nighttime => HoursToNextHour(DaytimeLengthInHours, hoursPassedToday), 
			_ => throw new ArgumentOutOfRangeException("timeOfDay", timeOfDay, null), 
		};
	}

	private static float HoursToNextHour(float hour, float hoursPassedToday)
	{
		if ((hour < 0f || hour > 24f) ? true : false)
		{
			throw new ArgumentException($"Hour {hour} is outside of the accepted range of 0 to 24");
		}
		if (!(hoursPassedToday < hour))
		{
			return hour + 24f - hoursPassedToday;
		}
		return hour - hoursPassedToday;
	}

	private void InitializeConfigurationValues()
	{
		_dayNightCycleSpec = _specService.GetSingleSpec<DayNightCycleSpec>();
		DaytimeLengthInHours = _dayNightCycleSpec.ConfiguredDaytimeLengthInHours;
		_daytimeLengthInTicks = HoursToTicks(DaytimeLengthInHours);
		NighttimeLengthInHours = 24f - DaytimeLengthInHours;
		DayLengthInSeconds = (float)_dayNightCycleSpec.ConfiguredDayLengthInTicks * _tickService.TickIntervalInSeconds;
		FixedDeltaTimeInHours = SecondsToHours(_tickService.TickIntervalInSeconds);
	}

	private void ProgressTimeBy(int deltaTicks)
	{
		TimeOfDay timeOfDay = TimeOfDay;
		_ticksPassedToday += deltaTicks;
		if (_ticksPassedToday >= _dayNightCycleSpec.ConfiguredDayLengthInTicks)
		{
			_ticksPassedToday -= _dayNightCycleSpec.ConfiguredDayLengthInTicks;
			int dayNumber = DayNumber + 1;
			DayNumber = dayNumber;
		}
		if (timeOfDay != TimeOfDay)
		{
			PostTimeOfDayEvent();
		}
	}

	private void PostTimeOfDayEvent()
	{
		switch (TimeOfDay)
		{
		case TimeOfDay.Daytime:
			_eventBus.Post(new DaytimeStartEvent());
			break;
		case TimeOfDay.Nighttime:
			_eventBus.Post(new NighttimeStartEvent());
			break;
		default:
			throw new ArgumentException($"Unexpected: {TimeOfDay}");
		}
	}
}
