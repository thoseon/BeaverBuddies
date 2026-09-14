using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.MapStateSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.GameCycleSystem;

public class GameCycleService : ISaveableSingleton, ILoadableSingleton
{
	private static readonly SingletonKey GameCycleServiceKey = new SingletonKey("GameCycleService");

	private static readonly PropertyKey<int> CycleKey = new PropertyKey<int>("Cycle");

	private static readonly PropertyKey<int> CycleDayKey = new PropertyKey<int>("CycleDay");

	private readonly EventBus _eventBus;

	private readonly ISingletonLoader _singletonLoader;

	private readonly IDayNightCycle _dayNightCycle;

	private readonly MapEditorMode _mapEditorMode;

	private readonly ImmutableArray<ICycleDuration> _cycleDurations;

	private int _cycleDurationInDays;

	public int Cycle { get; private set; }

	public int CycleDay { get; private set; }

	public float PartialCycleDay => (float)CycleDay + _dayNightCycle.DayProgress;

	public GameCycleService(EventBus eventBus, ISingletonLoader singletonLoader, IDayNightCycle dayNightCycle, MapEditorMode mapEditorMode, IEnumerable<ICycleDuration> cycleDurations)
	{
		_eventBus = eventBus;
		_singletonLoader = singletonLoader;
		_dayNightCycle = dayNightCycle;
		_mapEditorMode = mapEditorMode;
		_cycleDurations = cycleDurations.ToImmutableArray();
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (!_mapEditorMode.IsMapEditor)
		{
			IObjectSaver singleton = singletonSaver.GetSingleton(GameCycleServiceKey);
			singleton.Set(CycleKey, Cycle);
			singleton.Set(CycleDayKey, CycleDay);
		}
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(GameCycleServiceKey, out var objectLoader))
		{
			Cycle = Math.Max(objectLoader.Get(CycleKey), 0);
			CycleDay = objectLoader.Get(CycleDayKey);
			_cycleDurationInDays = _cycleDurations.Sum((ICycleDuration duration) => duration.DurationInDays);
		}
		else
		{
			StartNextCycle();
		}
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnDaytimeStart(DaytimeStartEvent daytimeStartEvent)
	{
		StartNextDay();
	}

	private void StartNextDay()
	{
		int cycleDay = CycleDay + 1;
		CycleDay = cycleDay;
		if (CycleDay > _cycleDurationInDays)
		{
			StartNextCycle();
		}
		_eventBus.Post(new CycleDayStartedEvent());
	}

	private void StartNextCycle()
	{
		if (Cycle > 0)
		{
			_eventBus.Post(new CycleEndedEvent(Cycle));
		}
		int cycle = Cycle + 1;
		Cycle = cycle;
		CycleDay = 1;
		foreach (ICycleDuration cycleDuration in _cycleDurations)
		{
			cycleDuration.SetForCycle(Cycle);
		}
		_cycleDurationInDays = _cycleDurations.Sum((ICycleDuration duration) => duration.DurationInDays);
		_eventBus.Post(new CycleStartedEvent(Cycle));
	}
}
