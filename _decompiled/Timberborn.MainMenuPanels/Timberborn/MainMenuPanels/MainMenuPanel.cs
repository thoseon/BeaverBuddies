using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.ApplicationLifetime;
using Timberborn.CoreUI;
using Timberborn.ExperimentalModeSystem;
using Timberborn.GameSaveRepositorySystem;
using Timberborn.GameSaveRepositorySystemUI;
using Timberborn.Localization;
using Timberborn.MainMenuModdingUI;
using Timberborn.MapRepositorySystemUI;
using Timberborn.SettingsSystemUI;
using Timberborn.SingletonSystem;
using Timberborn.StoreSystem;
using Timberborn.TooltipSystem;
using Timberborn.WebNavigation;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.MainMenuPanels;

public class MainMenuPanel : IPanelController, ILoadableSingleton
{
	private static readonly string UpdateInfoHeaderLocKey = "MainMenu.UpdateInfoHeader";

	private static readonly string UpdateInfoHeaderExperimentalLocKey = "MainMenu.UpdateInfoHeaderExperimental";

	private readonly GameSaveRepository _gameSaveRepository;

	private readonly LoadGameBox _loadGameBox;

	private readonly LoadMapBox _loadMapBox;

	private readonly NewGameFactionPanel _newGameFactionPanel;

	private readonly NewMapBox _newMapBox;

	private readonly ISettingsController _settingsController;

	private readonly CreditsBox _creditsBox;

	private readonly PanelStack _panelStack;

	private readonly UrlOpener _urlOpener;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ExperimentalMode _experimentalMode;

	private readonly ValidatingGameLoader _validatingGameLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly IStore _store;

	private readonly ILoc _loc;

	private readonly MainMenuSoundController _mainMenuSoundController;

	private readonly ModManagerBox _modManagerBox;

	private VisualElement _root;

	private Button _continueButton;

	public MainMenuPanel(GameSaveRepository gameSaveRepository, LoadGameBox loadGameBox, LoadMapBox loadMapBox, NewGameFactionPanel newGameFactionPanel, NewMapBox newMapBox, ISettingsController settingsController, CreditsBox creditsBox, PanelStack panelStack, UrlOpener urlOpener, VisualElementLoader visualElementLoader, ExperimentalMode experimentalMode, ValidatingGameLoader validatingGameLoader, ITooltipRegistrar tooltipRegistrar, IStore store, ILoc loc, MainMenuSoundController mainMenuSoundController, ModManagerBox modManagerBox)
	{
		_gameSaveRepository = gameSaveRepository;
		_loadGameBox = loadGameBox;
		_loadMapBox = loadMapBox;
		_newGameFactionPanel = newGameFactionPanel;
		_newMapBox = newMapBox;
		_settingsController = settingsController;
		_creditsBox = creditsBox;
		_panelStack = panelStack;
		_urlOpener = urlOpener;
		_visualElementLoader = visualElementLoader;
		_experimentalMode = experimentalMode;
		_validatingGameLoader = validatingGameLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_store = store;
		_loc = loc;
		_mainMenuSoundController = mainMenuSoundController;
		_modManagerBox = modManagerBox;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("MainMenu/MainMenuPanel");
		_root.Q<Button>("NewGameButton").RegisterCallback<ClickEvent>(NewGameClicked);
		_root.Q<Button>("LoadGameButton").RegisterCallback<ClickEvent>(LoadGameClicked);
		_root.Q<Button>("NewMapButton").RegisterCallback<ClickEvent>(NewMapClicked);
		_root.Q<Button>("LoadMapButton").RegisterCallback<ClickEvent>(LoadMapClicked);
		_root.Q<Button>("ModManagerButton").RegisterCallback<ClickEvent>(ModManagerClicked);
		_root.Q<Button>("SettingsButton").RegisterCallback<ClickEvent>(SettingsClicked);
		_root.Q<Button>("FeedbackButton").RegisterCallback<ClickEvent>(FeedbackClicked);
		_root.Q<Button>("ExitButton").RegisterCallback<ClickEvent>(ExitClicked);
		_root.Q<Button>("CreditsButton").RegisterCallback<ClickEvent>(CreditsClicked);
		_root.Q<Label>("Experimental").ToggleDisplayStyle(_experimentalMode.IsExperimental);
		_continueButton = _root.Q<Button>("ContinueButton");
		_continueButton.RegisterCallback<ClickEvent>(ContinueClicked);
		Button button = _root.Q<Button>("DiscordButton");
		button.RegisterCallback<ClickEvent>(DiscordClicked);
		_tooltipRegistrar.Register(button, UrlOpener.DiscordUrl);
		Button button2 = _root.Q<Button>("MerchandiseButton");
		button2.RegisterCallback<ClickEvent>(MerchandiseClicked);
		_tooltipRegistrar.Register(button2, UrlOpener.MerchandiseUrl);
		Button button3 = _root.Q<Button>("UpdateInfoLink");
		if (string.IsNullOrEmpty(_store.FullUpdateUrl))
		{
			button3.ToggleDisplayStyle(visible: false);
		}
		else
		{
			button3.RegisterCallback<ClickEvent>(UpdateInfoLinkClicked);
			_tooltipRegistrar.Register(button3, _store.ShortUpdateUrl ?? _store.FullUpdateUrl);
		}
		_root.Q<Label>("UpdateInfoHeader").text = (_experimentalMode.IsExperimental ? _loc.T(UpdateInfoHeaderExperimentalLocKey) : _loc.T(UpdateInfoHeaderLocKey));
		_root.Q<Label>("UpdateInfoText").text = GetBulletListWithIndentation(_loc.T(_store.UpdateInfoTextLocKey));
	}

	public void Show()
	{
		_continueButton.ToggleDisplayStyle(_gameSaveRepository.GetMostRecentSave() != null);
		_panelStack.Push(this);
		_mainMenuSoundController.PlayThemeMusic();
		Time.timeScale = 1f;
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public bool OnUIConfirmed()
	{
		return false;
	}

	public void OnUICancelled()
	{
	}

	private void ContinueClicked(ClickEvent evt)
	{
		LoadMostRecentSave();
	}

	private void NewGameClicked(ClickEvent evt)
	{
		_panelStack.HideAndPush(_newGameFactionPanel);
	}

	private void LoadGameClicked(ClickEvent evt)
	{
		_loadGameBox.Open();
	}

	private void NewMapClicked(ClickEvent evt)
	{
		_panelStack.HideAndPush(_newMapBox);
	}

	private void LoadMapClicked(ClickEvent evt)
	{
		_loadMapBox.Open();
	}

	private void ModManagerClicked(ClickEvent evt)
	{
		_modManagerBox.Open();
	}

	private void SettingsClicked(ClickEvent evt)
	{
		_panelStack.HideAndPush(_settingsController);
	}

	private void CreditsClicked(ClickEvent evt)
	{
		_panelStack.HideAndPush(_creditsBox);
	}

	private void FeedbackClicked(ClickEvent evt)
	{
		_urlOpener.OpenFeatureUpvote();
	}

	private static void ExitClicked(ClickEvent evt)
	{
		GameQuitter.Quit();
	}

	private void DiscordClicked(ClickEvent evt)
	{
		_urlOpener.OpenDiscord();
	}

	private void MerchandiseClicked(ClickEvent evt)
	{
		_urlOpener.OpenMerchandise();
	}

	private void UpdateInfoLinkClicked(ClickEvent evt)
	{
		_urlOpener.OpenUrl(_store.FullUpdateUrl);
	}

	private void LoadMostRecentSave()
	{
		SaveReference mostRecentSave = _gameSaveRepository.GetMostRecentSave();
		LoadSaveIfExists(mostRecentSave);
	}

	private void LoadSaveIfExists(SaveReference saveReference)
	{
		if (_gameSaveRepository.SaveExists(saveReference))
		{
			_validatingGameLoader.LoadGame(saveReference);
		}
		else
		{
			Debug.LogWarning($"Save: {saveReference} doesn't exist, failed to load.");
		}
	}

	private static string GetBulletListWithIndentation(string inputText)
	{
		IEnumerable<string> values = from line in inputText.Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
			select "<indent=-8px>" + line + "</indent>";
		return string.Join(Environment.NewLine, values);
	}
}
