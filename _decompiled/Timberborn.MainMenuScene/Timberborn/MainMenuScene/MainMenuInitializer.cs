using Timberborn.AnalyticsUI;
using Timberborn.Intro;
using Timberborn.MainMenuPanels;
using Timberborn.MainMenuSceneLoading;
using Timberborn.SceneLoading;
using Timberborn.SingletonSystem;
using Timberborn.StoreSystem;
using Timberborn.TitleScreenUI;

namespace Timberborn.MainMenuScene;

internal class MainMenuInitializer : ILoadableSingleton
{
	private readonly TitleScreen _titleScreen;

	private readonly AutoStarter _autoStarter;

	private readonly IStore _store;

	private readonly WelcomeScreenBox _welcomeScreenBox;

	private readonly MacOSPermissionsChecker _macOSPermissionsChecker;

	private readonly InitialLanguageChooser _initialLanguageChooser;

	private readonly PlayerDataLoader _playerDataLoader;

	private readonly MainMenuPanel _mainMenuPanel;

	private readonly ISceneLoader _sceneLoader;

	private readonly TitleScreenFooter _titleScreenFooter;

	private readonly AnalyticsConsentBox _analyticsConsentBox;

	private readonly IntroBox _introBox;

	public MainMenuInitializer(TitleScreen titleScreen, AutoStarter autoStarter, IStore store, WelcomeScreenBox welcomeScreenBox, MacOSPermissionsChecker macOSPermissionsChecker, InitialLanguageChooser initialLanguageChooser, PlayerDataLoader playerDataLoader, MainMenuPanel mainMenuPanel, ISceneLoader sceneLoader, TitleScreenFooter titleScreenFooter, AnalyticsConsentBox analyticsConsentBox, IntroBox introBox)
	{
		_titleScreen = titleScreen;
		_autoStarter = autoStarter;
		_store = store;
		_welcomeScreenBox = welcomeScreenBox;
		_macOSPermissionsChecker = macOSPermissionsChecker;
		_initialLanguageChooser = initialLanguageChooser;
		_playerDataLoader = playerDataLoader;
		_mainMenuPanel = mainMenuPanel;
		_sceneLoader = sceneLoader;
		_titleScreenFooter = titleScreenFooter;
		_analyticsConsentBox = analyticsConsentBox;
		_introBox = introBox;
	}

	public void Load()
	{
		if (_store.GameIsAllowedToStart)
		{
			_titleScreen.Initialize();
			if (!_sceneLoader.TryGetSceneParameters<MainMenuSceneParameters>(out var sceneParameters))
			{
				_macOSPermissionsChecker.CheckPermissions(LoadPlayerSettings);
			}
			else if (sceneParameters.ShowWelcomeScreen)
			{
				ShowWelcomeScreen();
			}
			else
			{
				ShowMainMenuPanel();
			}
		}
	}

	private void LoadPlayerSettings()
	{
		_playerDataLoader.Load(CheckAutoStarting);
	}

	private void CheckAutoStarting()
	{
		_autoStarter.CheckAutoStarting(ShowIntroScreen);
	}

	private void ShowIntroScreen()
	{
		_introBox.Show(CheckInitialLanguage);
	}

	private void CheckInitialLanguage()
	{
		_initialLanguageChooser.CheckInitialLanguage(CheckAnalyticsConsent);
	}

	private void CheckAnalyticsConsent()
	{
		_analyticsConsentBox.Show(ShowWelcomeScreen);
	}

	private void ShowWelcomeScreen()
	{
		_welcomeScreenBox.Show(ShowMainMenuPanel);
		_titleScreenFooter.Show();
	}

	private void ShowMainMenuPanel()
	{
		_mainMenuPanel.Show();
		_titleScreenFooter.Show();
	}
}
