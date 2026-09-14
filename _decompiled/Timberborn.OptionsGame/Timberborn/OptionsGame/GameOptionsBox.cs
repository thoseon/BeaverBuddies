using Timberborn.CoreUI;
using Timberborn.ExperimentalModeSystem;
using Timberborn.GameExitSystem;
using Timberborn.GameSaveRepositorySystemUI;
using Timberborn.GameSaveRuntimeSystemUI;
using Timberborn.KeyBindingSystemUI;
using Timberborn.Options;
using Timberborn.SettingsSystemUI;
using Timberborn.SingletonSystem;
using Timberborn.Versioning;
using Timberborn.WebNavigation;
using UnityEngine.UIElements;

namespace Timberborn.OptionsGame;

public class GameOptionsBox : IOptionsBox, IPanelController, IPanelBlocker, ILoadableSingleton
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly PanelStack _panelStack;

	private readonly UrlOpener _urlOpener;

	private readonly LoadGameBox _loadGameBox;

	private readonly ISettingsController _settingsController;

	private readonly KeyBindingsBox _keyBindingsBox;

	private readonly SaveGameBox _saveGameBox;

	private readonly GoodbyeBoxFactory _goodbyeBoxFactory;

	private readonly ExperimentalMode _experimentalMode;

	private VisualElement _root;

	public GameOptionsBox(VisualElementLoader visualElementLoader, PanelStack panelStack, UrlOpener urlOpener, LoadGameBox loadGameBox, ISettingsController settingsController, KeyBindingsBox keyBindingsBox, SaveGameBox saveGameBox, GoodbyeBoxFactory goodbyeBoxFactory, ExperimentalMode experimentalMode)
	{
		_visualElementLoader = visualElementLoader;
		_panelStack = panelStack;
		_urlOpener = urlOpener;
		_loadGameBox = loadGameBox;
		_settingsController = settingsController;
		_keyBindingsBox = keyBindingsBox;
		_saveGameBox = saveGameBox;
		_goodbyeBoxFactory = goodbyeBoxFactory;
		_experimentalMode = experimentalMode;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/GameOptionsBox");
		_root.Q<Button>("ResumeButton").RegisterCallback<ClickEvent>(ResumeClicked);
		_root.Q<Button>("SaveGameButton").RegisterCallback<ClickEvent>(SaveGameClicked);
		_root.Q<Button>("LoadGameButton").RegisterCallback<ClickEvent>(LoadGameClicked);
		_root.Q<Button>("BindingsButton").RegisterCallback<ClickEvent>(BindingsClicked);
		_root.Q<Button>("SettingsButton").RegisterCallback<ClickEvent>(SettingsClicked);
		_root.Q<Button>("FeedbackButton").RegisterCallback<ClickEvent>(FeedbackClicked);
		_root.Q<Button>("ExitToMenuButton").RegisterCallback<ClickEvent>(ExitToMenuClicked);
		_root.Q<Button>("ExitToDesktopButton").RegisterCallback<ClickEvent>(ExitToDesktopClicked);
		_root.Q<Button>("DiscordButton").RegisterCallback<ClickEvent>(DiscordClicked);
		_root.Q<Label>("GameVersion").text = GameVersions.CurrentVersion.Formatted;
		_root.Q<Label>("Experimental").ToggleDisplayStyle(_experimentalMode.IsExperimental);
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public void Show()
	{
		_panelStack.PushOverlay(this);
	}

	public bool OnUIConfirmed()
	{
		return false;
	}

	public void OnUICancelled()
	{
		_panelStack.Pop(this);
	}

	private void ResumeClicked(ClickEvent evt)
	{
		_panelStack.Pop(this);
	}

	private void SaveGameClicked(ClickEvent evt)
	{
		_saveGameBox.Open();
	}

	private void LoadGameClicked(ClickEvent evt)
	{
		_loadGameBox.Open();
	}

	private void BindingsClicked(ClickEvent evt)
	{
		_panelStack.HideAndPushOverlay(_keyBindingsBox);
	}

	private void SettingsClicked(ClickEvent evt)
	{
		_panelStack.HideAndPushOverlay(_settingsController);
	}

	private void FeedbackClicked(ClickEvent evt)
	{
		_urlOpener.OpenFeatureUpvote();
	}

	private void ExitToMenuClicked(ClickEvent evt)
	{
		_panelStack.HideAndPushOverlay(_goodbyeBoxFactory.ShowExitToMainMenu());
	}

	private void ExitToDesktopClicked(ClickEvent evt)
	{
		_panelStack.HideAndPushOverlay(_goodbyeBoxFactory.ShowExitToDesktop());
	}

	private void DiscordClicked(ClickEvent evt)
	{
		_urlOpener.OpenDiscord();
	}
}
