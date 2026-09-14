using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.GameSaveRepositorySystem;
using Timberborn.Localization;
using Timberborn.MapRepositorySystem;
using Timberborn.NewGameConfigurationSystem;
using Timberborn.SceneLoading;

namespace Timberborn.GameSceneLoading;

public class GameSceneLoader
{
	private readonly GameSaveRepository _gameSaveRepository;

	private readonly ILoc _loc;

	private readonly ISceneLoader _sceneLoader;

	private readonly GameModeSpecService _gameModeSpecService;

	private readonly ISpecService _specService;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	public GameSceneLoader(GameSaveRepository gameSaveRepository, ILoc loc, ISceneLoader sceneLoader, GameModeSpecService gameModeSpecService, ISpecService specService, IRandomNumberGenerator randomNumberGenerator)
	{
		_gameSaveRepository = gameSaveRepository;
		_loc = loc;
		_sceneLoader = sceneLoader;
		_gameModeSpecService = gameModeSpecService;
		_specService = specService;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void StartNewGame(NewGameConfiguration newGameConfiguration)
	{
		_sceneLoader.LoadScene(GameSceneParameters.CreateNewGameParameters(newGameConfiguration), GetTip());
	}

	public void StartSaveGame(SaveReference saveReference)
	{
		_sceneLoader.LoadScene(GameSceneParameters.CreateGameSaveParameters(saveReference), GetTip());
	}

	public void StartNewGameInstantly(string factionId, MapFileReference mapFileReference, string settlementName)
	{
		GameModeSpec defaultSpec = _gameModeSpecService.GetDefaultSpec();
		_sceneLoader.LoadSceneInstantly(GameSceneParameters.CreateNewGameParameters(new NewGameConfiguration(factionId, mapFileReference, defaultSpec, settlementName)), GetTip());
	}

	public void StartSaveGameInstantly(SaveReference saveReference)
	{
		_sceneLoader.LoadSceneInstantly(GameSceneParameters.CreateGameSaveParameters(saveReference), GetTip());
	}

	public void StartMostRecentSaveInstantly()
	{
		SaveReference mostRecentSave = _gameSaveRepository.GetMostRecentSave();
		if (_gameSaveRepository.SaveExists(mostRecentSave))
		{
			StartSaveGameInstantly(mostRecentSave);
		}
	}

	private string GetTip()
	{
		GameTipSpec singleSpec = _specService.GetSingleSpec<GameTipSpec>();
		string listElement = _randomNumberGenerator.GetListElement(singleSpec.Tips);
		return _loc.T(listElement);
	}
}
