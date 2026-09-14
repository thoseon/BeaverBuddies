using Timberborn.Buildings;
using Timberborn.Common;
using Timberborn.GameSceneLoading;
using Timberborn.NewGameConfigurationSystem;
using Timberborn.SceneLoading;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;
using Timberborn.UILayoutSystem;
using UnityEngine;

namespace Timberborn.GameStartup;

internal class GameInitializer : IUpdatableSingleton
{
	private enum InitializationState
	{
		Waiting,
		SpawnBeavers,
		PostSpawnBeavers,
		UnpauseGame,
		ShowUI,
		Finished
	}

	private readonly StartingBuildingSpawner _startingBuildingSpawner;

	private readonly StartingBeaversInitializer _startingBeaverInitializer;

	private readonly ISceneLoader _sceneLoader;

	private readonly SpeedManager _speedManager;

	private readonly EventBus _eventBus;

	private InitializationState _initializationState;

	public bool IsGameInitialized => _initializationState == InitializationState.Finished;

	public GameInitializer(StartingBuildingSpawner startingBuildingSpawner, StartingBeaversInitializer startingBeaverInitializer, ISceneLoader sceneLoader, SpeedManager speedManager, EventBus eventBus)
	{
		_startingBuildingSpawner = startingBuildingSpawner;
		_startingBeaverInitializer = startingBeaverInitializer;
		_sceneLoader = sceneLoader;
		_speedManager = speedManager;
		_eventBus = eventBus;
	}

	public void InitializeNewGame()
	{
		_initializationState = InitializationState.SpawnBeavers;
	}

	public void InitializeGameFromSave()
	{
		_initializationState = InitializationState.ShowUI;
	}

	public void UpdateSingleton()
	{
		if (_initializationState != InitializationState.Finished)
		{
			Initialize();
		}
	}

	private void Initialize()
	{
		_initializationState = PerformNextInitializationStep();
	}

	private InitializationState PerformNextInitializationStep()
	{
		return _initializationState switch
		{
			InitializationState.SpawnBeavers => SpawnBeavers(), 
			InitializationState.PostSpawnBeavers => PostSpawnBeavers(), 
			InitializationState.UnpauseGame => UnpauseGame(), 
			InitializationState.ShowUI => ShowPrimaryUI(), 
			_ => _initializationState, 
		};
	}

	private InitializationState SpawnBeavers()
	{
		Building startingBuilding = _startingBuildingSpawner.StartingBuilding;
		if ((bool)startingBuilding)
		{
			Vector3? unblockedSingleAccess = startingBuilding.GetComponent<BuildingAccessible>().Accessible.UnblockedSingleAccess;
			if (unblockedSingleAccess.HasValue)
			{
				Vector3 valueOrDefault = unblockedSingleAccess.GetValueOrDefault();
				GameModeSpec gameMode = _sceneLoader.GetSceneParameters<GameSceneParameters>().NewGameConfiguration.GameMode;
				_startingBeaverInitializer.Initialize(valueOrDefault, gameMode.StartingAdults, gameMode.AdultAgeProgress, gameMode.StartingChildren, gameMode.ChildAgeProgress);
			}
		}
		return InitializationState.PostSpawnBeavers;
	}

	private InitializationState PostSpawnBeavers()
	{
		_eventBus.Post(new NewGameInitializedEvent());
		return InitializationState.UnpauseGame;
	}

	private InitializationState UnpauseGame()
	{
		_speedManager.ChangeSpeed(1f);
		return InitializationState.ShowUI;
	}

	private InitializationState ShowPrimaryUI()
	{
		_eventBus.Post(new ShowPrimaryUIEvent());
		return InitializationState.Finished;
	}
}
