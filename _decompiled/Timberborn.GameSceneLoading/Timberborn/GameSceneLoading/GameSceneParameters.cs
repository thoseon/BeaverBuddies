using Timberborn.GameSaveRepositorySystem;
using Timberborn.NewGameConfigurationSystem;
using Timberborn.SceneLoading;

namespace Timberborn.GameSceneLoading;

public class GameSceneParameters : ISceneParameters
{
	public NewGameConfiguration NewGameConfiguration { get; }

	public SaveReference SaveReference { get; }

	public bool NewGame => NewGameConfiguration != null;

	public int SceneIndex => 2;

	private GameSceneParameters(NewGameConfiguration newGameConfiguration, SaveReference saveReference)
	{
		NewGameConfiguration = newGameConfiguration;
		SaveReference = saveReference;
	}

	public static GameSceneParameters CreateNewGameParameters(NewGameConfiguration newGameConfiguration)
	{
		return new GameSceneParameters(newGameConfiguration, null);
	}

	public static GameSceneParameters CreateGameSaveParameters(SaveReference saveReference)
	{
		return new GameSceneParameters(null, saveReference);
	}
}
