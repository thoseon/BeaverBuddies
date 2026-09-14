using System;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.MapEditorSceneLoading;
using Timberborn.MapRepositorySystem;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.MapRepositorySystemUI;

public class LoadMapBox : IPanelController, ILoadableSingleton
{
	private static readonly string DeleteMapPromptLocKey = "LoadMapPanel.DeleteMapPrompt";

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly ILoc _loc;

	private readonly MapEditorSceneLoader _mapEditorSceneLoader;

	private readonly MapValidator _mapValidator;

	private readonly PanelStack _panelStack;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly MapSelection _mapSelection;

	private readonly MapRepository _mapRepository;

	private Button _deleteMap;

	private Button _load;

	private VisualElement _root;

	public LoadMapBox(DialogBoxShower dialogBoxShower, ILoc loc, MapEditorSceneLoader mapEditorSceneLoader, MapValidator mapValidator, PanelStack panelStack, VisualElementLoader visualElementLoader, MapSelection mapSelection, MapRepository mapRepository)
	{
		_dialogBoxShower = dialogBoxShower;
		_loc = loc;
		_mapEditorSceneLoader = mapEditorSceneLoader;
		_mapValidator = mapValidator;
		_panelStack = panelStack;
		_visualElementLoader = visualElementLoader;
		_mapSelection = mapSelection;
		_mapRepository = mapRepository;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Options/LoadMapBox");
		_deleteMap = _root.Q<Button>("DeleteMap");
		_deleteMap.RegisterCallback<ClickEvent>(OnDeleteMapButtonClicked);
		_load = _root.Q<Button>("LoadButton");
		_load.RegisterCallback<ClickEvent>(delegate
		{
			TryLoadMap();
		});
		_root.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(delegate
		{
			OnUICancelled();
		});
		_mapSelection.InitializeWithMapGoalsHidden(_root, delegate
		{
			TryLoadMap();
		});
		_mapSelection.SelectedMapChanged += OnSelectionChanged;
	}

	public void Open()
	{
		_panelStack.HideAndPush(this);
		_mapSelection.Open();
	}

	public void OpenAsOverlay()
	{
		_panelStack.HideAndPushOverlay(this);
		_mapSelection.Open();
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public bool OnUIConfirmed()
	{
		return TryLoadMap();
	}

	public void OnUICancelled()
	{
		_mapSelection.Clear();
		_panelStack.Pop(this);
	}

	private void OnDeleteMapButtonClicked(ClickEvent evt)
	{
		if (_mapSelection.TryGetSelectedMap(out var mapItem) && mapItem.IsDeletable)
		{
			_dialogBoxShower.Create().SetMessage(_loc.T(DeleteMapPromptLocKey, mapItem.DisplayName)).SetConfirmButton(delegate
			{
				_mapRepository.DeleteMap(mapItem.MapFileReference);
			})
				.SetDefaultCancelButton()
				.Show();
		}
	}

	private void OnSelectionChanged(object sender, EventArgs eventArgs)
	{
		if (_mapSelection.TryGetSelectedMap(out var selectedMap))
		{
			_deleteMap.SetEnabled(selectedMap.IsDeletable);
			_load.SetEnabled(value: true);
		}
		else
		{
			_deleteMap.SetEnabled(value: false);
			_load.SetEnabled(value: false);
		}
	}

	private bool TryLoadMap()
	{
		if (_mapSelection.TryGetSelectedMap(out var selectedMap))
		{
			MapFileReference mapFileReference = selectedMap.MapFileReference;
			_mapValidator.ValidateForMapEditor(mapFileReference, delegate
			{
				_mapEditorSceneLoader.LoadMap(mapFileReference);
			});
			return true;
		}
		return false;
	}
}
