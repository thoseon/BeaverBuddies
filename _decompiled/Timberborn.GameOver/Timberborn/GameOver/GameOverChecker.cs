using System.Linq;
using Timberborn.Characters;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Population;
using Timberborn.Reproduction;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;

namespace Timberborn.GameOver;

internal class GameOverChecker : IGameOverChecker, ILoadableSingleton, ITickableSingleton, IPostLoadableSingleton
{
	private static readonly int FailTickDelay = 10;

	private readonly EventBus _eventBus;

	private readonly EntityComponentRegistry _entityComponentRegistry;

	private readonly EntityRegistry _entityRegistry;

	private readonly PopulationService _populationService;

	private readonly GameOverDisabler _gameOverDisabler;

	private bool _gameEnded;

	private int _ticksElapsed;

	public GameOverChecker(EventBus eventBus, EntityComponentRegistry entityComponentRegistry, EntityRegistry entityRegistry, PopulationService populationService, GameOverDisabler gameOverDisabler)
	{
		_eventBus = eventBus;
		_entityComponentRegistry = entityComponentRegistry;
		_entityRegistry = entityRegistry;
		_populationService = populationService;
		_gameOverDisabler = gameOverDisabler;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public void PostLoad()
	{
		CheckGameEndedState();
	}

	[OnEvent]
	public void OnNewGameInitialized(NewGameInitializedEvent newGameInitializedEvent)
	{
		CheckGameEndedState();
	}

	public void Tick()
	{
		if (!_gameEnded && !_gameOverDisabler.Disabled)
		{
			_ticksElapsed = (IsGameOver() ? (_ticksElapsed + 1) : 0);
			if (_ticksElapsed == FailTickDelay)
			{
				_eventBus.Post(new GameOverEvent());
				_gameEnded = true;
			}
		}
	}

	public bool IsGameOver()
	{
		if (_populationService.AllDead && _entityRegistry.Entities.Count((EntityComponent entity) => entity.GetComponent<Character>()) == 0)
		{
			return !PhoenixProtocolActive();
		}
		return false;
	}

	private void CheckGameEndedState()
	{
		_gameEnded = _populationService.AllDead;
	}

	private bool PhoenixProtocolActive()
	{
		foreach (BreedingPod item in _entityComponentRegistry.GetEnabled<BreedingPod>())
		{
			if (item.HasResourcesToFinish())
			{
				return true;
			}
		}
		return false;
	}
}
