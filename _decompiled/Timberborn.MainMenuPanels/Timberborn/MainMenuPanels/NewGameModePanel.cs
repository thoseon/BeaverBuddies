using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.FactionSystem;
using Timberborn.GameSceneLoading;
using Timberborn.Localization;
using Timberborn.MapItemsUI;
using Timberborn.NewGameConfigurationSystem;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.MainMenuPanels;

public class NewGameModePanel : IPanelController, ILoadableSingleton
{
	private static readonly string StartLocKey = "NewGameConfigurationPanel.Start";

	private static readonly string CustomModeLocKey = "NewGameConfigurationPanel.Custom";

	private static readonly string SelectedModeClass = "new-game-mode-panel__mode--selected";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly GameSceneLoader _gameSceneLoader;

	private readonly PanelStack _panelStack;

	private readonly ILoc _loc;

	private readonly CustomNewGameModeController _customNewGameModeController;

	private readonly GameModeSpecService _gameModeSpecService;

	private readonly TutorialToggleController _tutorialToggleController;

	private ImmutableArray<GameModeSpec> _gameModeSpecs;

	private MapItem _map;

	private FactionSpec _factionSpec;

	private GameModeSpec _predefinedGameMode;

	private VisualElement _root;

	private Label _summary;

	private Button _nextButton;

	private readonly List<Button> _modeButtons = new List<Button>();

	private Button _customizeButton;

	private Button _selectedModeButton;

	private Label _modeDescription;

	private VisualElement _customModeSettings;

	private bool _visible;

	public NewGameModePanel(VisualElementLoader visualElementLoader, GameSceneLoader gameSceneLoader, PanelStack panelStack, ILoc loc, CustomNewGameModeController customNewGameModeController, GameModeSpecService gameModeSpecService, TutorialToggleController tutorialToggleController)
	{
		_visualElementLoader = visualElementLoader;
		_gameSceneLoader = gameSceneLoader;
		_panelStack = panelStack;
		_loc = loc;
		_customNewGameModeController = customNewGameModeController;
		_gameModeSpecService = gameModeSpecService;
		_tutorialToggleController = tutorialToggleController;
	}

	public void Load()
	{
		_gameModeSpecs = _gameModeSpecService.GetSpecsOrdered();
		GameModeSpec defaultSpec = _gameModeSpecService.GetDefaultSpec();
		_root = _visualElementLoader.LoadVisualElement("MainMenu/NewGameModePanel");
		_tutorialToggleController.Initialize(_root);
		_summary = _root.Q<Label>("SummaryText");
		_nextButton = _root.Q<Button>("NextButton");
		_nextButton.RegisterCallback<ClickEvent>(delegate
		{
			StartNewGame();
		});
		_nextButton.text = _loc.T(StartLocKey);
		Button button = _root.Q<Button>("BackButton");
		button.RegisterCallback<ClickEvent>(delegate
		{
			OnUICancelled();
		});
		button.text = _loc.T(CommonLocKeys.NavigationBackKey);
		_modeDescription = _root.Q<Label>("ModeDescription");
		_customModeSettings = _root.Q("CustomModeSettings");
		_customizeButton = _root.Q<Button>("CustomizeButton");
		_customizeButton.RegisterCallback<ClickEvent>(delegate
		{
			OnCustomizeButtonClicked();
		});
		VisualElement visualElement = _root.Q<VisualElement>("Modes");
		foreach (GameModeSpec gameModeSpec in _gameModeSpecs)
		{
			Button modeButton = (Button)_visualElementLoader.LoadVisualElement("MainMenu/NewGameModeButton");
			modeButton.text = _loc.T(gameModeSpec.DisplayNameLocKey);
			modeButton.RegisterCallback<ClickEvent>(delegate
			{
				OnPredefinedModeButtonClicked(modeButton, gameModeSpec);
			});
			visualElement.Add(modeButton);
			_modeButtons.Add(modeButton);
			if (gameModeSpec == defaultSpec)
			{
				OnPredefinedModeButtonClicked(modeButton, gameModeSpec);
			}
		}
	}

	public void SelectFactionAndMap(FactionSpec factionSpec, MapItem map)
	{
		_map = map;
		_factionSpec = factionSpec;
	}

	public VisualElement GetPanel()
	{
		_visible = true;
		UpdateSummary();
		_tutorialToggleController.SetFaction(_factionSpec);
		return _root;
	}

	public bool OnUIConfirmed()
	{
		StartNewGame();
		return true;
	}

	public void OnUICancelled()
	{
		_visible = false;
		_panelStack.Pop(this);
	}

	private void StartNewGame()
	{
		if (TryGetValidatedGameMode(out var gameMode))
		{
			_gameSceneLoader.StartNewGame(new NewGameConfiguration(_factionSpec.Id, _map.MapFileReference, gameMode, string.Empty));
		}
	}

	private bool TryGetValidatedGameMode(out GameModeSpec gameMode)
	{
		if (_predefinedGameMode != null)
		{
			gameMode = _predefinedGameMode;
			return true;
		}
		return _customNewGameModeController.TryGetValidatedGameMode(out gameMode);
	}

	private void OnPredefinedModeButtonClicked(Button button, GameModeSpec predefinedGameMode)
	{
		_modeDescription.text = _loc.T(predefinedGameMode.DescriptionLocKey);
		_modeDescription.ToggleDisplayStyle(visible: true);
		_customModeSettings.ToggleDisplayStyle(visible: false);
		_customizeButton.ToggleDisplayStyle(visible: true);
		_tutorialToggleController.ShowMainToggle();
		SelectModeButton(button, predefinedGameMode);
	}

	private void OnCustomizeButtonClicked()
	{
		_modeDescription.ToggleDisplayStyle(visible: false);
		_customModeSettings.ToggleDisplayStyle(visible: true);
		_customizeButton.ToggleDisplayStyle(visible: false);
		_customNewGameModeController.Initialize(_root, _predefinedGameMode, UpdateNextButton);
		_tutorialToggleController.HideMainToggle();
		SelectModeButton(null, null);
	}

	private void SelectModeButton(Button button, GameModeSpec predefinedGameMode)
	{
		foreach (Button modeButton in _modeButtons)
		{
			modeButton.EnableInClassList(SelectedModeClass, modeButton == button);
		}
		_selectedModeButton = button;
		_predefinedGameMode = predefinedGameMode;
		UpdateSummary();
	}

	private void UpdateSummary()
	{
		if (_visible)
		{
			_summary.text = _factionSpec.DisplayName.Value + " - " + _map.DisplayName + " - " + (_selectedModeButton?.text ?? _loc.T(CustomModeLocKey));
			UpdateNextButton();
		}
	}

	private void UpdateNextButton()
	{
		_nextButton.SetEnabled(TryGetValidatedGameMode(out var _));
	}
}
