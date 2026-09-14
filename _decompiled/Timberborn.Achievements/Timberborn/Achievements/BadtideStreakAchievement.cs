using Timberborn.AchievementSystem;
using Timberborn.HazardousWeatherSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Achievements;

internal class BadtideStreakAchievement : Achievement, ILoadableSingleton, ISaveableSingleton
{
	private static readonly SingletonKey BadtideStreakAchievementKey = new SingletonKey("BadtideStreakAchievement");

	private static readonly PropertyKey<int> StreakCountKey = new PropertyKey<int>("StreakCount");

	private static readonly int RequiredBadtidesInRow = 2;

	private readonly ISingletonLoader _singletonLoader;

	private readonly EventBus _eventBus;

	private int _streakCount;

	public override string Id => "BADTIDE_STREAK";

	public BadtideStreakAchievement(ISingletonLoader singletonLoader, EventBus eventBus)
	{
		_singletonLoader = singletonLoader;
		_eventBus = eventBus;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (_streakCount > 0 && _streakCount < RequiredBadtidesInRow)
		{
			singletonSaver.GetSingleton(BadtideStreakAchievementKey).Set(StreakCountKey, _streakCount);
		}
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(BadtideStreakAchievementKey, out var objectLoader))
		{
			_streakCount = objectLoader.Get(StreakCountKey);
		}
	}

	[OnEvent]
	public void OnHazardousWeatherStarted(HazardousWeatherStartedEvent hazardousWeatherStartedEvent)
	{
		if (hazardousWeatherStartedEvent.HazardousWeather is BadtideWeather)
		{
			_streakCount++;
			if (_streakCount >= RequiredBadtidesInRow)
			{
				Unlock();
			}
		}
		else
		{
			_streakCount = 0;
		}
	}

	protected override void EnableInternal()
	{
		_eventBus.Register(this);
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
	}
}
