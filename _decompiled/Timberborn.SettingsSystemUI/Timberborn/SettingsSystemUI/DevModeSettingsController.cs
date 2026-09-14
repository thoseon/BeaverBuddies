using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.ErrorReporting;
using Timberborn.FactionSystem;
using Timberborn.GameSaveRepositorySystem;
using Timberborn.Localization;
using Timberborn.MainMenuSceneLoading;
using Timberborn.PlayerDataSystem;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.SettingsSystemUI;

internal class DevModeSettingsController : IUpdatableSingleton, IUnloadableSingleton
{
	private readonly DevModeManager _devModeManager;

	private readonly FactionUnlockingService _factionUnlockingService;

	private readonly ILocalizationService _localizationService;

	private readonly ILoc _loc;

	private readonly MainMenuSceneLoader _mainMenuSceneLoader;

	private readonly IPlayerDataService _playerDataService;

	private readonly GameSaveRepository _gameSaveRepository;

	private readonly EventBus _eventBus;

	private readonly PanelTextSettingsUpdater _panelTextSettingsUpdater;

	private VisualElement _wrapper;

	private Button _testExceptionButton;

	private Label _testLabel;

	private bool _languageTestInProgress;

	private readonly Dictionary<string, Queue<string>> _textsToTest = new Dictionary<string, Queue<string>>();

	private string _currentLanguage;

	public DevModeSettingsController(DevModeManager devModeManager, FactionUnlockingService factionUnlockingService, ILocalizationService localizationService, ILoc loc, MainMenuSceneLoader mainMenuSceneLoader, IPlayerDataService playerDataService, GameSaveRepository gameSaveRepository, EventBus eventBus, PanelTextSettingsUpdater panelTextSettingsUpdater)
	{
		_devModeManager = devModeManager;
		_factionUnlockingService = factionUnlockingService;
		_localizationService = localizationService;
		_loc = loc;
		_mainMenuSceneLoader = mainMenuSceneLoader;
		_playerDataService = playerDataService;
		_gameSaveRepository = gameSaveRepository;
		_eventBus = eventBus;
		_panelTextSettingsUpdater = panelTextSettingsUpdater;
	}

	public void Initialize(VisualElement root, Action cancelAction)
	{
		_wrapper = root.Q<VisualElement>("Developer");
		_testLabel = root.Q<Label>("DeveloperTestLabel");
		_testLabel.ToggleDisplayStyle(visible: false);
		root.Q<VisualElement>("Developer").style.display = DisplayStyle.Flex;
		root.Q<Button>("LockFactions").RegisterCallback<ClickEvent>(delegate
		{
			_factionUnlockingService.LockAllFactions();
		});
		root.Q<Button>("UnlockFactions").RegisterCallback<ClickEvent>(delegate
		{
			_factionUnlockingService.UnlockAllFactions();
		});
		root.Q<Button>("ClearPlayerPrefs").RegisterCallback<ClickEvent>(delegate
		{
			OnClearPlayerPrefsClick(cancelAction);
		});
		root.Q<Button>("TestLanguages").RegisterCallback<ClickEvent>(delegate
		{
			OnTestLanguagesClick();
		});
		_testExceptionButton = root.Q<Button>("TestException");
		_testExceptionButton.RegisterCallback<ClickEvent>(delegate
		{
			OnTestExceptionClick();
		});
		root.Q<Button>("ShowCleanLog").RegisterCallback<ClickEvent>(delegate
		{
			OnShowCleanLogClick();
		});
		_eventBus.Register(this);
	}

	public void Unload()
	{
		_eventBus.Unregister((object)this);
	}

	public void Update()
	{
		_wrapper.ToggleDisplayStyle(_devModeManager.Enabled);
		_testExceptionButton.ToggleDisplayStyle(_gameSaveRepository.DevelopmentSettlementExists());
		_testLabel.ToggleDisplayStyle(visible: false);
	}

	public void UpdateSingleton()
	{
		if (_textsToTest.Count > 0)
		{
			_testLabel.ToggleDisplayStyle(visible: true);
			if (_textsToTest[_currentLanguage].Count == 0)
			{
				_textsToTest.Remove(_currentLanguage);
				_currentLanguage = _textsToTest.Keys.FirstOrDefault();
				if (!string.IsNullOrEmpty(_currentLanguage))
				{
					_panelTextSettingsUpdater.Update(_currentLanguage);
				}
			}
			if (!string.IsNullOrEmpty(_currentLanguage))
			{
				_testLabel.text = DequeueText();
				while (_textsToTest[_currentLanguage].Count > 0 && _testLabel.text.Length < 5000)
				{
					_testLabel.text += " ";
					_testLabel.text += DequeueText();
				}
			}
		}
		else if (_languageTestInProgress)
		{
			_mainMenuSceneLoader.SaveAndOpenMainMenu();
		}
	}

	[OnEvent]
	public void OnDevModeToggledEvent(DevModeToggledEvent _)
	{
		Update();
	}

	private void OnClearPlayerPrefsClick(Action cancelAction)
	{
		PlayerPrefs.DeleteAll();
		_playerDataService.RemoveAll();
		cancelAction();
	}

	private void OnTestLanguagesClick()
	{
		string currentLanguage = _localizationService.CurrentLanguage;
		foreach (LanguageInfo availableLanguage in _localizationService.AvailableLanguages)
		{
			_localizationService.Load(availableLanguage.LocalizationCode);
			_textsToTest[availableLanguage.LocalizationCode] = new Queue<string>();
			foreach (string rawText in _loc.GetRawTexts())
			{
				_textsToTest[availableLanguage.LocalizationCode].Enqueue(rawText);
			}
		}
		_localizationService.Load(currentLanguage);
		_wrapper.parent.parent.parent.ToggleDisplayStyle(visible: false);
		_currentLanguage = _textsToTest.Keys.First();
		_languageTestInProgress = true;
	}

	private string DequeueText()
	{
		return _textsToTest[_currentLanguage].Dequeue().Replace(Environment.NewLine, " ");
	}

	private void OnTestExceptionClick()
	{
		_devModeManager.Disable();
		throw new Exception("Test");
	}

	private void OnShowCleanLogClick()
	{
		_testLabel.ToggleDisplayStyle(visible: true);
		_testLabel.text = PlayerLogCleaner.GetCleanedPlayerLog();
	}
}
