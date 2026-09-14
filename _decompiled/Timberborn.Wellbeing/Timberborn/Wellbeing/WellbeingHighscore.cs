using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Wellbeing;

internal class WellbeingHighscore : ITickableSingleton, ISaveableSingleton, ILoadableSingleton
{
	private static readonly SingletonKey WellbeingHighscoreKey = new SingletonKey("WellbeingHighscore");

	private static readonly PropertyKey<int> AverageWellbeingHighscoreKey = new PropertyKey<int>("AverageWellbeingHighscore");

	private readonly ISingletonLoader _singletonLoader;

	private readonly WellbeingService _wellbeingService;

	private readonly EventBus _eventBus;

	private int _averageWellbeingHighscore;

	private int _tickCounter;

	public WellbeingHighscore(ISingletonLoader singletonLoader, WellbeingService wellbeingService, EventBus eventBus)
	{
		_singletonLoader = singletonLoader;
		_wellbeingService = wellbeingService;
		_eventBus = eventBus;
	}

	public void Tick()
	{
		if (_wellbeingService.AverageGlobalWellbeing > _averageWellbeingHighscore)
		{
			_averageWellbeingHighscore = _wellbeingService.AverageGlobalWellbeing;
			if (_tickCounter++ > 1)
			{
				_eventBus.Post(new NewWellbeingHighscoreEvent(_averageWellbeingHighscore));
			}
		}
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(WellbeingHighscoreKey, out var objectLoader))
		{
			_averageWellbeingHighscore = objectLoader.Get(AverageWellbeingHighscoreKey);
		}
		_eventBus.Register(this);
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		singletonSaver.GetSingleton(WellbeingHighscoreKey).Set(AverageWellbeingHighscoreKey, _averageWellbeingHighscore);
	}
}
