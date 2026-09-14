using System;
using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.MapItemsUI;
using Timberborn.MapRepositorySystem;
using Timberborn.PlatformUtilities;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.MapRepositorySystemUI;

public class MapSelection
{
	private static readonly string SelectedClass = "selected";

	private readonly MapItemElementFactory _mapItemElementFactory;

	private readonly MapItemProvider _mapItemProvider;

	private readonly SelectedMapPanel _selectedMapPanel;

	private readonly EventBus _eventBus;

	private readonly IExplorerOpener _explorerOpener;

	private readonly MapDownloader _mapDownloader;

	private ListView _mapList;

	private Button _officialMapsButton;

	private Button _customMapsButton;

	private Button _downloadButton;

	private Button _browseButton;

	private Label _emptyLabel;

	private readonly List<MapItem> _maps = new List<MapItem>();

	private bool _officialMapsShown;

	private Action _doubleClickAction;

	public event EventHandler SelectedMapChanged;

	public MapSelection(MapItemElementFactory mapItemElementFactory, MapItemProvider mapItemProvider, SelectedMapPanel selectedMapPanel, EventBus eventBus, IExplorerOpener explorerOpener, MapDownloader mapDownloader)
	{
		_mapItemElementFactory = mapItemElementFactory;
		_mapItemProvider = mapItemProvider;
		_selectedMapPanel = selectedMapPanel;
		_eventBus = eventBus;
		_explorerOpener = explorerOpener;
		_mapDownloader = mapDownloader;
	}

	public void InitializeWithMapGoalsShown(VisualElement root, Action doubleClickAction)
	{
		Initialize(root, showMapGoals: true, doubleClickAction);
	}

	public void InitializeWithMapGoalsHidden(VisualElement root, Action doubleClickAction)
	{
		Initialize(root, showMapGoals: false, doubleClickAction);
	}

	public void Open()
	{
		_selectedMapPanel.Open();
		ShowOfficialMaps();
		_eventBus.Register(this);
	}

	public void Clear()
	{
		_mapItemElementFactory.Clear();
		_maps.Clear();
		_mapList.Clear();
		_mapList.ClearSelection();
		_selectedMapPanel.ClearSelection();
		_eventBus.Unregister(this);
	}

	public bool TryGetSelectedMap(out MapItem selectedMap)
	{
		selectedMap = _mapList.selectedItem as MapItem;
		return selectedMap != null;
	}

	[OnEvent]
	public void OnMapRepositoryChanged(MapRepositoryChangedEvent mapRepositoryChangedEvent)
	{
		MapItem selectedMap = _mapList.selectedItem as MapItem;
		if (_officialMapsShown)
		{
			ShowOfficialMaps();
		}
		else
		{
			ShowCustomMaps();
		}
		if (selectedMap != null)
		{
			int num = _maps.FindIndex((MapItem map) => map.MapFileReference.Equals(selectedMap.MapFileReference));
			if (num >= 0)
			{
				_mapList.SetSelection(num);
			}
		}
	}

	private void Initialize(VisualElement root, bool showMapGoals, Action doubleClickAction)
	{
		_mapList = root.Q<ListView>("MapList");
		_mapList.makeItem = CreateAndBind;
		_mapList.bindItem = delegate(VisualElement ve, int i)
		{
			_mapItemElementFactory.Bind(ve, _maps[i], showMapGoals);
		};
		_mapList.itemsSource = _maps;
		_doubleClickAction = doubleClickAction;
		_mapList.selectionChanged += OnSelectionChanged;
		_mapList.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
		if (showMapGoals)
		{
			_selectedMapPanel.InitializeWithFlexibleStartInfoShown(root);
		}
		else
		{
			_selectedMapPanel.InitializeWithFlexibleStartInfoHidden(root);
		}
		_officialMapsButton = root.Q<Button>("OfficialMapsButton");
		_officialMapsButton.RegisterCallback<ClickEvent>(delegate
		{
			ShowOfficialMaps();
		});
		_customMapsButton = root.Q<Button>("CustomMapsButton");
		_customMapsButton.RegisterCallback<ClickEvent>(delegate
		{
			ShowCustomMaps();
		});
		_downloadButton = root.Q<Button>("DownloadButton");
		_downloadButton.RegisterCallback<ClickEvent>(OnDownloadClicked);
		_browseButton = root.Q<Button>("BrowseButton");
		_browseButton.RegisterCallback<ClickEvent>(OnBrowseClicked);
		_emptyLabel = root.Q<Label>("EmptyText");
	}

	private VisualElement CreateAndBind()
	{
		VisualElement visualElement = _mapItemElementFactory.Create();
		visualElement.RegisterCallback<ClickEvent>(OnClickMapItem);
		return visualElement;
	}

	private void OnClickMapItem(ClickEvent clickEvent)
	{
		if (clickEvent.clickCount == 2)
		{
			_doubleClickAction();
		}
	}

	private void OnSelectionChanged(IEnumerable<object> obj)
	{
		if (TryGetSelectedMap(out var selectedMap))
		{
			_selectedMapPanel.Update(selectedMap);
		}
		else
		{
			_selectedMapPanel.ClearSelection();
		}
		SelectedMapChanged?.Invoke(this, EventArgs.Empty);
	}

	private void ShowOfficialMaps()
	{
		ShowMaps(_mapItemProvider.GetOfficialMaps());
		UpdateVisualElements(officialMapsShown: true);
	}

	private void ShowCustomMaps()
	{
		ShowMaps(_mapItemProvider.GetCustomMaps());
		UpdateVisualElements(officialMapsShown: false);
	}

	private void ShowMaps(IEnumerable<MapItem> mapsToShow)
	{
		_maps.Clear();
		_maps.AddRange(mapsToShow);
		_mapList.RefreshItems();
		_mapList.ClearSelection();
		_mapList.SetSelection(0);
		_mapList.ScrollToItem(0);
	}

	private void UpdateVisualElements(bool officialMapsShown)
	{
		_officialMapsShown = officialMapsShown;
		_officialMapsButton.EnableInClassList(SelectedClass, _officialMapsShown);
		_customMapsButton.EnableInClassList(SelectedClass, !_officialMapsShown);
		_emptyLabel.ToggleDisplayStyle(_maps.Count == 0);
		_downloadButton.ToggleDisplayStyle(!officialMapsShown && _mapDownloader.HasDownloader);
		_browseButton.ToggleDisplayStyle(!officialMapsShown);
	}

	private void OnDownloadClicked(ClickEvent evt)
	{
		_mapDownloader.Download();
	}

	private void OnBrowseClicked(ClickEvent evt)
	{
		_explorerOpener.OpenDirectory(MapRepository.UserMapsDirectory);
	}
}
