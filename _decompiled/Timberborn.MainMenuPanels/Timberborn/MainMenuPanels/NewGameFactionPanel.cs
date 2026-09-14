using System;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.FactionSystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.MainMenuPanels;

public class NewGameFactionPanel : IPanelController, ILoadableSingleton
{
	private readonly FactionSpecService _factionSpecService;

	private readonly NewGameMapPanel _newGameMapPanel;

	private readonly PanelStack _panelStack;

	private readonly ILoc _loc;

	private readonly MainMenuSoundController _mainMenuSoundController;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly FactionUnlockingService _factionUnlockingService;

	private readonly FactionUnlockConditionDescriber _factionUnlockConditionDescriber;

	private VisualElement _root;

	private VisualElement _selectedFactionElement;

	private FactionSpec _selectedFactionSpec;

	private VisualElement _factionList;

	private ImmutableArray<FactionSpec> _factionSpecs;

	private Button _next;

	private Label _unlockCondition;

	public NewGameFactionPanel(FactionSpecService factionSpecService, NewGameMapPanel newGameMapPanel, PanelStack panelStack, ILoc loc, MainMenuSoundController mainMenuSoundController, VisualElementLoader visualElementLoader, FactionUnlockingService factionUnlockingService, FactionUnlockConditionDescriber factionUnlockConditionDescriber)
	{
		_factionSpecService = factionSpecService;
		_newGameMapPanel = newGameMapPanel;
		_panelStack = panelStack;
		_loc = loc;
		_mainMenuSoundController = mainMenuSoundController;
		_visualElementLoader = visualElementLoader;
		_factionUnlockingService = factionUnlockingService;
		_factionUnlockConditionDescriber = factionUnlockConditionDescriber;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("MainMenu/NewGameFactionPanel");
		_next = _root.Q<Button>("NextButton");
		_next.RegisterCallback<ClickEvent>(OnNextButtonClicked);
		_next.text = _loc.T(CommonLocKeys.NavigationNextKey);
		Button button = _root.Q<Button>("BackButton");
		button.RegisterCallback<ClickEvent>(delegate
		{
			OnUICancelled();
		});
		button.text = _loc.T(CommonLocKeys.NavigationBackKey);
		_unlockCondition = _root.Q<Label>("UnlockCondition");
		_factionSpecs = _factionSpecService.Factions.OrderBy((FactionSpec faction) => faction.Order).ToImmutableArray();
		CreateFactions(_root);
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public bool OnUIConfirmed()
	{
		return ShowNewGameMap();
	}

	public void OnUICancelled()
	{
		_panelStack.Pop(this);
	}

	private void CreateFactions(VisualElement root)
	{
		_factionList = root.Q<VisualElement>("FactionList");
		for (int i = 0; i < _factionSpecs.Length; i++)
		{
			FactionSpec factionSpec = _factionSpecs[i];
			VisualElement visualElement = CreateFaction(factionSpec);
			AssignArrowButtons(i, visualElement);
			_factionList.Add(visualElement);
			if (i == 0)
			{
				SelectFaction(factionSpec, visualElement, playSound: false);
			}
		}
	}

	private VisualElement CreateFaction(FactionSpec factionSpec)
	{
		VisualElement factionElement = _visualElementLoader.LoadVisualElement("MainMenu/NewGameFactionItem");
		factionElement.Q<VisualElement>("NormalFaction").RegisterCallback<ClickEvent>(delegate
		{
			SelectFaction(factionSpec, factionElement, playSound: true);
		});
		factionElement.RegisterCallback(delegate(KeyUpEvent evt)
		{
			if (evt.keyCode == KeyCode.Return)
			{
				SelectFaction(factionSpec, factionElement, playSound: true);
			}
		});
		Sprite asset = factionSpec.NewGameFullAvatar.Asset;
		Sprite asset2 = factionSpec.Logo.Asset;
		SetBackground(factionElement, "Avatar", asset);
		SetBackground(factionElement, "Logo", asset2);
		SetBackground(factionElement, "SelectedLogo", asset2);
		SetBackground(factionElement, "DescriptionLogo", asset2);
		SetBackground(factionElement, "SelectedAvatar", asset);
		factionElement.Q<Label>("SelectedFactionName").text = factionSpec.DisplayName.Value;
		factionElement.Q<Label>("SelectedFactionDescription").text = factionSpec.Description.Value;
		return factionElement;
	}

	private void AssignArrowButtons(int index, VisualElement element)
	{
		int previous = Math.Max(0, index - 1);
		element.Q<Button>("LeftArrow").RegisterCallback<ClickEvent>(delegate
		{
			SelectFaction(_factionSpecs[previous], _factionList[previous], playSound: true);
		});
		int next = Math.Min(_factionSpecs.Length - 1, index + 1);
		element.Q<Button>("RightArrow").RegisterCallback<ClickEvent>(delegate
		{
			SelectFaction(_factionSpecs[next], _factionList[next], playSound: true);
		});
	}

	private void SelectFaction(FactionSpec factionSpec, VisualElement factionElement, bool playSound)
	{
		if (_selectedFactionElement != factionElement)
		{
			_selectedFactionElement?.RemoveFromClassList("selected-faction");
			_selectedFactionElement?.AddToClassList("normal-faction");
			_selectedFactionElement = factionElement;
			_selectedFactionElement.RemoveFromClassList("normal-faction");
			_selectedFactionElement.AddToClassList("selected-faction");
			_selectedFactionSpec = factionSpec;
			bool flag = _factionUnlockingService.IsLocked(factionSpec);
			_next.SetEnabled(!flag);
			_unlockCondition.visible = flag;
			_unlockCondition.text = _factionUnlockConditionDescriber.Describe(factionSpec);
			int num = 352 + (_factionSpecs.Length - 1) * 156;
			int num2 = _factionList.IndexOf(factionElement);
			_factionList.style.marginLeft = new StyleLength(num - 352 - 156 * num2 * 2);
			if (playSound)
			{
				_mainMenuSoundController.PlayFactionSelectedSound(factionSpec);
			}
		}
	}

	private void OnNextButtonClicked(ClickEvent evt)
	{
		ShowNewGameMap();
	}

	private bool ShowNewGameMap()
	{
		if (_selectedFactionSpec != null && !_factionUnlockingService.IsLocked(_selectedFactionSpec))
		{
			_newGameMapPanel.Open(_selectedFactionSpec);
			return true;
		}
		return false;
	}

	private static void SetBackground(VisualElement factionElement, string name, Sprite sprite)
	{
		factionElement.Q<VisualElement>(name).style.backgroundImage = new StyleBackground(sprite);
	}
}
