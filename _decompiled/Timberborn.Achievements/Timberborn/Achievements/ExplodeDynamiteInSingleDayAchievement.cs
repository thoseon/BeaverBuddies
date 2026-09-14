using Timberborn.AchievementSystem;
using Timberborn.Explosions;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Achievements;

internal class ExplodeDynamiteInSingleDayAchievement : Achievement, ILoadableSingleton, ISaveableSingleton
{
	private static readonly SingletonKey DynamiteExplodedInSingleDayAchievementKey = new SingletonKey("DynamiteExplodedInSingleDayAchievement");

	private static readonly PropertyKey<int> DetonationCountKey = new PropertyKey<int>("DetonationCount");

	private static readonly int DynamiteToDetonate = 200;

	private readonly EventBus _eventBus;

	private readonly ISingletonLoader _singletonLoader;

	private int _detonationCount;

	public override string Id => "EXPLODE_DYNAMITE_IN_SINGLE_DAY";

	public ExplodeDynamiteInSingleDayAchievement(EventBus eventBus, ISingletonLoader singletonLoader)
	{
		_eventBus = eventBus;
		_singletonLoader = singletonLoader;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (_detonationCount > 0 && _detonationCount < DynamiteToDetonate)
		{
			singletonSaver.GetSingleton(DynamiteExplodedInSingleDayAchievementKey).Set(DetonationCountKey, _detonationCount);
		}
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(DynamiteExplodedInSingleDayAchievementKey, out var objectLoader))
		{
			_detonationCount = objectLoader.Get(DetonationCountKey);
		}
	}

	[OnEvent]
	public void OnDaytimeStartEvent(DaytimeStartEvent daytimeStartEvent)
	{
		_detonationCount = 0;
	}

	[OnEvent]
	public void OnDynamiteDetonated(DynamiteDetonatedEvent dynamiteDetonatedEvent)
	{
		_detonationCount++;
		if (_detonationCount >= DynamiteToDetonate)
		{
			Unlock();
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
