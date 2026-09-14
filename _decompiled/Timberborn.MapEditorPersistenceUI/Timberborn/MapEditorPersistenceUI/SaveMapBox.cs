using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.MapItemsUI;
using Timberborn.MapRepositorySystem;
using Timberborn.PlatformUtilities;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.MapEditorPersistenceUI;

public class SaveMapBox : IPanelController, ILoadableSingleton
{
	private static readonly string HeaderLocKey = "MapEditor.SaveMap.Header";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly MapItemProvider _mapItemProvider;

	private readonly PanelStack _panelStack;

	private readonly IExplorerOpener _explorerOpener;

	private readonly MapPersistenceController _mapPersistenceController;

	private readonly ILoc _loc;

	private readonly InputService _inputService;

	private VisualElement _root;

	private ListView _mapList;

	private TextField _mapName;

	private Button _save;

	private readonly List<MapItem> _maps = new List<MapItem>();

	private Action _successAction;

	private bool MapNameValid => !string.IsNullOrWhiteSpace(_mapName.value);

	public SaveMapBox(VisualElementLoader visualElementLoader, MapItemProvider mapItemProvider, PanelStack panelStack, IExplorerOpener explorerOpener, MapPersistenceController mapPersistenceController, ILoc loc, InputService inputService)
	{
		_visualElementLoader = visualElementLoader;
		_mapItemProvider = mapItemProvider;
		_panelStack = panelStack;
		_explorerOpener = explorerOpener;
		_mapPersistenceController = mapPersistenceController;
		_loc = loc;
		_inputService = inputService;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Options/SaveBox");
		_root.Q<Label>("Header").text = _loc.T(HeaderLocKey);
		_mapName = _root.Q<TextField>("SaveName");
		_mapName.maxLength = 50;
		_mapName.focusable = true;
		_mapName.RegisterCallback<ChangeEvent<string>>(delegate
		{
			UpdateSaveButton();
		});
		_mapName.Q<TextElement>().SetConfirmCancelActions(_inputService, SaveMap, OnUICancelled);
		_mapList = _root.Q<ListView>("ItemList");
		_mapList.makeItem = CreateAndBind;
		_mapList.bindItem = delegate(VisualElement ve, int i)
		{
			ve.Q<Label>("Text").text = _maps[i].MapFileReference.Name;
		};
		_mapList.itemsSource = _maps;
		_mapList.selectionChanged += InsertName;
		_mapList.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
		_save = _root.Q<Button>("SaveButton");
		_save.RegisterCallback<ClickEvent>(OnSaveButtonClicked);
		_save.SetEnabled(value: false);
		_root.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(delegate
		{
			OnUICancelled();
		});
		_root.Q<Button>("BrowseDirectoryButton").RegisterCallback<ClickEvent>(OnBrowseDirectoryButtonClicked);
	}

	public void Open(Action successAction)
	{
		_successAction = successAction;
		_panelStack.HideAndPushOverlay(this);
		_mapList.ClearSelection();
		_mapList.ScrollToItem(0);
		_mapName.Focus();
	}

	public VisualElement GetPanel()
	{
		_maps.AddRange(_mapItemProvider.GetUserMaps());
		_mapList.RefreshItems();
		return _root;
	}

	public bool OnUIConfirmed()
	{
		if (MapNameValid)
		{
			SaveMap();
			return true;
		}
		return false;
	}

	public void OnUICancelled()
	{
		Close();
	}

	private VisualElement CreateAndBind()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Options/ListViewItem");
		visualElement.RegisterCallback<ClickEvent>(OnMapListElementClick);
		return visualElement;
	}

	private void OnMapListElementClick(ClickEvent evt)
	{
		if (evt.clickCount == 2)
		{
			SaveMap();
		}
	}

	private void UpdateSaveButton()
	{
		_save.SetEnabled(MapNameValid);
	}

	private void OnSaveButtonClicked(ClickEvent evt)
	{
		SaveMap();
	}

	private void InsertName(IEnumerable<object> obj)
	{
		if (obj.SingleOrDefault() is MapItem { MapFileReference: var mapFileReference })
		{
			if (mapFileReference.Resource)
			{
				throw new ArgumentException("Unexpected resource map.");
			}
			_mapName.SetValueWithoutNotify(mapFileReference.Name);
			_save.SetEnabled(value: true);
		}
	}

	private void OnBrowseDirectoryButtonClicked(ClickEvent evt)
	{
		_explorerOpener.OpenDirectory(MapRepository.UserMapsDirectory);
	}

	private void SaveMap()
	{
		string value = _mapName.value;
		if (MapNameValid)
		{
			_mapPersistenceController.SaveAs(value, MapSavedCallback);
		}
	}

	private void MapSavedCallback()
	{
		Action successAction = _successAction;
		Close();
		successAction?.Invoke();
	}

	private void Close()
	{
		_mapName.value = string.Empty;
		UpdateSaveButton();
		_maps.Clear();
		_successAction = null;
		_panelStack.Pop(this);
	}
}
