using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.GameDistricts;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsUI;

internal class DistrictListPanel
{
	private static readonly int ListViewItemHeight = 31;

	private static readonly int MaxListViewHeight = 155;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly EventBus _eventBus;

	private readonly List<DistrictCenter> _districts = new List<DistrictCenter>();

	private VisualElement _root;

	private ListView _districtListView;

	public DistrictListPanel(VisualElementLoader visualElementLoader, EntitySelectionService entitySelectionService, EventBus eventBus)
	{
		_visualElementLoader = visualElementLoader;
		_entitySelectionService = entitySelectionService;
		_eventBus = eventBus;
	}

	public void Initialize(VisualElement root)
	{
		_root = root.Q("DistrictListPanel");
		InitializeDistrictList();
		Hide();
		_eventBus.Register(this);
	}

	public void UpdateDistrictList()
	{
		_districtListView.RefreshItems();
	}

	public void Show()
	{
		_root.ToggleDisplayStyle(visible: true);
	}

	public void Hide()
	{
		_root.ToggleDisplayStyle(visible: false);
	}

	[OnEvent]
	public void OnDistrictSelected(DistrictSelectedEvent districtSelectedEvent)
	{
		SelectOnList(districtSelectedEvent.DistrictCenter);
	}

	[OnEvent]
	public void OnDistrictUnselected(DistrictUnselectedEvent districtUnselectedEvent)
	{
		_districtListView.ClearSelection();
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		DistrictCenter component = enteredFinishedStateEvent.BlockObject.GetComponent<DistrictCenter>();
		if ((bool)component)
		{
			_districts.Add(component);
			UpdateDistrictListAndHeight();
		}
	}

	[OnEvent]
	public void OnExitedFinishedState(ExitedFinishedStateEvent exitedFinishedStateEvent)
	{
		DistrictCenter component = exitedFinishedStateEvent.BlockObject.GetComponent<DistrictCenter>();
		if ((bool)component)
		{
			_districts.Remove(component);
			UpdateDistrictListAndHeight();
		}
	}

	private void SelectOnList(DistrictCenter districtCenter)
	{
		int num = _districts.IndexOf(districtCenter);
		_districtListView.SetSelectionWithoutNotify(Enumerables.One(num));
		_districtListView.ScrollToItem(num);
	}

	private void InitializeDistrictList()
	{
		_districtListView = _root.Q<ListView>("DistrictList");
		_districtListView.makeItem = () => _visualElementLoader.LoadVisualElement("Game/Districts/DistrictListPanelItem");
		_districtListView.bindItem = delegate(VisualElement ve, int i)
		{
			ve.Q<Label>("Text").text = _districts[i].DistrictName;
		};
		_districtListView.itemsSource = _districts;
		_districtListView.selectionChanged += OnDistrictListSelectionChanged;
		_districtListView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
		UpdateDistrictListHeight();
	}

	private void OnDistrictListSelectionChanged(IEnumerable<object> obj)
	{
		DistrictCenter districtCenter = (DistrictCenter)obj.SingleOrDefault();
		if ((bool)districtCenter)
		{
			_entitySelectionService.SelectAndFocusOn(districtCenter);
		}
	}

	private void UpdateDistrictListAndHeight()
	{
		UpdateDistrictListHeight();
		UpdateDistrictList();
	}

	private void UpdateDistrictListHeight()
	{
		int num = Math.Min(_districts.Count * ListViewItemHeight, MaxListViewHeight);
		_districtListView.style.height = num;
	}
}
