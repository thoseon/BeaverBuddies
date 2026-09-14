using Timberborn.SceneLoading;

namespace Timberborn.MainMenuSceneLoading;

public class MainMenuSceneParameters : ISceneParameters
{
	public bool ShowWelcomeScreen { get; }

	public int SceneIndex => 1;

	private MainMenuSceneParameters(bool showWelcomeScreen)
	{
		ShowWelcomeScreen = showWelcomeScreen;
	}

	public static MainMenuSceneParameters CreateWithWelcomeScreen()
	{
		return new MainMenuSceneParameters(showWelcomeScreen: true);
	}

	public static MainMenuSceneParameters CreateWithoutWelcomeScreen()
	{
		return new MainMenuSceneParameters(showWelcomeScreen: false);
	}
}
