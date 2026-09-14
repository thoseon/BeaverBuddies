using Timberborn.SceneLoading;
using Timberborn.SingletonSystem;

namespace Timberborn.MainMenuSceneLoading;

public class MainMenuSceneLoader
{
	private readonly EventBus _eventBus;

	private readonly ISceneLoader _sceneLoader;

	public MainMenuSceneLoader(ISceneLoader sceneLoader, EventBus eventBus)
	{
		_sceneLoader = sceneLoader;
		_eventBus = eventBus;
	}

	public void SaveAndOpenMainMenu()
	{
		_eventBus.Post(new PreMainMenuStartedEvent(skipAutoSave: false));
		_sceneLoader.LoadSceneInstantly(CreateMainMenuSceneParameters());
	}

	public void OpenMainMenu()
	{
		_eventBus.Post(new PreMainMenuStartedEvent(skipAutoSave: true));
		_sceneLoader.LoadSceneInstantly(CreateMainMenuSceneParameters());
	}

	private MainMenuSceneParameters CreateMainMenuSceneParameters()
	{
		if (!_sceneLoader.HasAnySceneParameters() || _sceneLoader.TryGetSceneParameters<MainMenuSceneParameters>(out var _))
		{
			return MainMenuSceneParameters.CreateWithWelcomeScreen();
		}
		return MainMenuSceneParameters.CreateWithoutWelcomeScreen();
	}
}
