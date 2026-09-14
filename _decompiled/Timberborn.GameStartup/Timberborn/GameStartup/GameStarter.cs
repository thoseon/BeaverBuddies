using Timberborn.GameSaveRuntimeSystem;
using Timberborn.SettlementNameSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.GameStartup;

internal class GameStarter : IUpdatableSingleton, ILoadableSingleton
{
	private readonly SettlementReferenceService _settlementReferenceService;

	private readonly GameInitializer _gameInitializer;

	private readonly StartingBuildingInitializer _startingBuildingInitializer;

	private readonly ISettlementNamePromptShower _settlementNamePromptShower;

	private readonly EventBus _eventBus;

	private readonly GameLoader _gameLoader;

	private bool _shouldSpawnStartingBuilding;

	public GameStarter(SettlementReferenceService settlementReferenceService, GameInitializer gameInitializer, StartingBuildingInitializer startingBuildingInitializer, ISettlementNamePromptShower settlementNamePromptShower, EventBus eventBus, GameLoader gameLoader)
	{
		_settlementReferenceService = settlementReferenceService;
		_gameInitializer = gameInitializer;
		_startingBuildingInitializer = startingBuildingInitializer;
		_settlementNamePromptShower = settlementNamePromptShower;
		_eventBus = eventBus;
		_gameLoader = gameLoader;
	}

	public void Load()
	{
		if (_gameLoader.IsNewGame)
		{
			_shouldSpawnStartingBuilding = true;
		}
		else
		{
			StartGameplay(forNewGame: false);
		}
	}

	public void UpdateSingleton()
	{
		if (_shouldSpawnStartingBuilding)
		{
			_shouldSpawnStartingBuilding = false;
			SpawnStartingBuilding();
		}
	}

	[OnEvent]
	public void OnSettlementNameChanged(SettlementNameChangedEvent settlementNameChangedEvent)
	{
		_settlementReferenceService.InitializeAndLogSettlementName(settlementNameChangedEvent.SettlementName);
		StartGameplay(forNewGame: true);
	}

	private void SpawnStartingBuilding()
	{
		_startingBuildingInitializer.Initialize();
		if (_settlementReferenceService.SettlementReference == null)
		{
			_eventBus.Register(this);
			_settlementNamePromptShower.PromptDisallowingCancelling(includeResetStartLocationLink: false);
		}
		else
		{
			StartGameplay(forNewGame: true);
		}
	}

	private void StartGameplay(bool forNewGame)
	{
		_eventBus.Unregister(this);
		if (forNewGame)
		{
			_gameInitializer.InitializeNewGame();
		}
		else
		{
			_gameInitializer.InitializeGameFromSave();
		}
	}
}
