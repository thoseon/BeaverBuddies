using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.GameWonderCompletion;

public class WonderCompletionCountdownStarter : ITickableSingleton, ILoadableSingleton, ISaveableSingleton
{
	private static readonly float UnlockOffsetInHours = 0.5f;

	private static readonly SingletonKey WonderCompletionCountdownStarterKey = new SingletonKey("WonderCompletionCountdownStarter");

	private static readonly PropertyKey<bool> CountdownFinishedKey = new PropertyKey<bool>("CountdownFinished");

	private static readonly PropertyKey<float> UnlockDayKey = new PropertyKey<float>("UnlockDay");

	private readonly GameWonderCompletionService _gameWonderCompletionService;

	private readonly EventBus _eventBus;

	private readonly IDayNightCycle _dayNightCycle;

	private readonly ISingletonLoader _singletonLoader;

	private float _unlockDay = float.MaxValue;

	public bool CountdownFinished { get; private set; }

	public WonderCompletionCountdownStarter(GameWonderCompletionService gameWonderCompletionService, EventBus eventBus, IDayNightCycle dayNightCycle, ISingletonLoader singletonLoader)
	{
		_gameWonderCompletionService = gameWonderCompletionService;
		_eventBus = eventBus;
		_dayNightCycle = dayNightCycle;
		_singletonLoader = singletonLoader;
	}

	public void Tick()
	{
		if (!CountdownFinished && _unlockDay < _dayNightCycle.PartialDayNumber)
		{
			_gameWonderCompletionService.CompleteWonder();
			_eventBus.Post(new WonderCompletedEvent());
			CountdownFinished = true;
		}
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(WonderCompletionCountdownStarterKey, out var objectLoader))
		{
			CountdownFinished = objectLoader.Get(CountdownFinishedKey);
			_unlockDay = objectLoader.Get(UnlockDayKey);
		}
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		IObjectSaver singleton = singletonSaver.GetSingleton(WonderCompletionCountdownStarterKey);
		singleton.Set(CountdownFinishedKey, CountdownFinished);
		singleton.Set(UnlockDayKey, _unlockDay);
	}

	public void BeginUnlockCountdown()
	{
		if (_unlockDay == float.MaxValue)
		{
			_eventBus.Post(new WonderCompletionCountdownStartedEvent());
			_unlockDay = _dayNightCycle.DayNumberHoursFromNow(UnlockOffsetInHours);
		}
	}
}
