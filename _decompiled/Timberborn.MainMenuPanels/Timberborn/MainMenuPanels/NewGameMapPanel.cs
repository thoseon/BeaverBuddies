using System;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.FactionSystem;
using Timberborn.Localization;
using Timberborn.MapItemsUI;
using Timberborn.MapRepositorySystemUI;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.MainMenuPanels;

public class NewGameMapPanel : IPanelController, ILoadableSingleton
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly PanelStack _panelStack;

	private readonly NewGameModePanel _newGameModePanel;

	private readonly MapValidator _mapValidator;

	private readonly MapSelection _mapSelection;

	private FactionSpec _factionSpec;

	private VisualElement _root;

	private Button _next;

	public NewGameMapPanel(VisualElementLoader visualElementLoader, ILoc loc, PanelStack panelStack, NewGameModePanel newGameModePanel, MapValidator mapValidator, MapSelection mapSelection)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_panelStack = panelStack;
		_newGameModePanel = newGameModePanel;
		_mapValidator = mapValidator;
		_mapSelection = mapSelection;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("MainMenu/NewGameMapPanel");
		_next = _root.Q<Button>("NextButton");
		_next.RegisterCallback<ClickEvent>(OnNextButtonClicked);
		_next.text = _loc.T(CommonLocKeys.NavigationNextKey);
		Button button = _root.Q<Button>("BackButton");
		button.RegisterCallback<ClickEvent>(delegate
		{
			OnUICancelled();
		});
		button.text = _loc.T(CommonLocKeys.NavigationBackKey);
		_mapSelection.InitializeWithMapGoalsShown(_root, delegate
		{
			OnNextButtonClicked(null);
		});
		_mapSelection.SelectedMapChanged += OnSelectionChanged;
	}

	public void Open(FactionSpec factionSpec)
	{
		_factionSpec = factionSpec;
		_panelStack.HideAndPush(this);
		_mapSelection.Open();
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public bool OnUIConfirmed()
	{
		ShowParametersPanelIfMapValid();
		return false;
	}

	public void OnUICancelled()
	{
		Clear();
		_panelStack.Pop(this);
	}

	private void Clear()
	{
		_mapSelection.Clear();
	}

	private void OnNextButtonClicked(ClickEvent evt)
	{
		ShowParametersPanelIfMapValid();
	}

	private void OnSelectionChanged(object sender, EventArgs e)
	{
		_next.SetEnabled(_mapSelection.TryGetSelectedMap(out var _));
	}

	private void ShowParametersPanelIfMapValid()
	{
		if (_mapSelection.TryGetSelectedMap(out var selectedMap))
		{
			_mapValidator.ValidateForNewGame(selectedMap.MapFileReference, delegate
			{
				ShowParametersPanel(selectedMap);
			});
		}
	}

	private void ShowParametersPanel(MapItem mapItem)
	{
		_newGameModePanel.SelectFactionAndMap(_factionSpec, mapItem);
		_panelStack.HideAndPush(_newGameModePanel);
	}
}
