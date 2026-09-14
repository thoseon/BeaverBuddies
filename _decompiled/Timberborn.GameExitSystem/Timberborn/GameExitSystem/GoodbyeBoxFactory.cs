using System;
using Timberborn.ApplicationLifetime;
using Timberborn.CoreUI;
using Timberborn.MainMenuSceneLoading;
using Timberborn.WebNavigation;

namespace Timberborn.GameExitSystem;

public class GoodbyeBoxFactory
{
	private readonly MainMenuSceneLoader _mainMenuSceneLoader;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly PanelStack _panelStack;

	private readonly UrlOpener _urlOpener;

	public GoodbyeBoxFactory(MainMenuSceneLoader mainMenuSceneLoader, VisualElementLoader visualElementLoader, PanelStack panelStack, UrlOpener urlOpener)
	{
		_mainMenuSceneLoader = mainMenuSceneLoader;
		_visualElementLoader = visualElementLoader;
		_panelStack = panelStack;
		_urlOpener = urlOpener;
	}

	public GoodbyeBox ShowExitToDesktop()
	{
		return GetController(GameQuitter.Quit);
	}

	public GoodbyeBox ShowExitToMainMenu()
	{
		return GetController(_mainMenuSceneLoader.SaveAndOpenMainMenu);
	}

	private GoodbyeBox GetController(Action action)
	{
		return new GoodbyeBox(_visualElementLoader, _panelStack, _urlOpener, action);
	}
}
