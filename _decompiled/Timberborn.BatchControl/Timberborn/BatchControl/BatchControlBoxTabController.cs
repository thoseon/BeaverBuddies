using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Timberborn.AssetSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.TooltipSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.BatchControl;

internal class BatchControlBoxTabController : IUpdatableSingleton, ILateUpdatableSingleton
{
	private static readonly string SpriteDirectory = "Sprites/BatchControl";

	private static readonly string ActiveButtonClass = "batch-control-panel__tab-button--active";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly IAssetLoader _assetLoader;

	private readonly ILoc _loc;

	private readonly EventBus _eventBus;

	private readonly EntityRegistry _entityRegistry;

	private readonly BatchControlBoxTimeRangeController _batchControlBoxTimeRangeController;

	private readonly ImmutableArray<BatchControlModule> _batchControlModules;

	private readonly Dictionary<BatchControlTab, VisualElement> _tabs = new Dictionary<BatchControlTab, VisualElement>();

	private readonly List<EntityComponent> _entities = new List<EntityComponent>();

	private VisualElement _tabButtons;

	private VisualElement _content;

	private VisualElement _middleRow;

	private Label _header;

	public BatchControlTab CurrentTab { get; private set; }

	public int LastOpenedTabIndex { get; private set; }

	public IEnumerable<BatchControlTab> Tabs => _tabs.Keys;

	public BatchControlBoxTabController(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, IAssetLoader assetLoader, ILoc loc, EventBus eventBus, EntityRegistry entityRegistry, IEnumerable<BatchControlModule> batchControlModules, BatchControlBoxTimeRangeController batchControlBoxTimeRangeController)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_assetLoader = assetLoader;
		_loc = loc;
		_eventBus = eventBus;
		_entityRegistry = entityRegistry;
		_batchControlBoxTimeRangeController = batchControlBoxTimeRangeController;
		_batchControlModules = batchControlModules.ToImmutableArray();
	}

	public void Initialize(VisualElement root)
	{
		_tabButtons = root.Q<VisualElement>("TabButtons");
		_content = root.Q<VisualElement>("Content");
		_middleRow = root.Q<VisualElement>("MiddleRow");
		_header = root.Q<Label>("Header");
		_batchControlBoxTimeRangeController.Initialize(root);
		AddTabs();
	}

	public void UpdateEntities()
	{
		_entities.AddRange(_entityRegistry.Entities);
	}

	public int GetTabIndex(BatchControlTab batchControlTab)
	{
		return _tabs.Keys.IndexOf(batchControlTab);
	}

	public void ShowTab(int index)
	{
		BatchControlTab batchControlTab = _tabs.Keys.ElementAt(index);
		if (batchControlTab != CurrentTab)
		{
			SetNewTab(batchControlTab);
			UpdateActiveButtonClass(index);
			LastOpenedTabIndex = index;
			_eventBus.Post(new BatchControlTabShownEvent(batchControlTab));
			CurrentTab.UpdateRowsVisibility();
		}
	}

	public void UpdateSingleton()
	{
		BatchControlTab currentTab = CurrentTab;
		if (currentTab != null && currentTab.IsDirty)
		{
			Refresh();
		}
	}

	public void LateUpdateSingleton()
	{
		CurrentTab?.UpdateContent();
	}

	public void Clear()
	{
		foreach (BatchControlTab item in _tabs.Keys.ToList())
		{
			item.Clear();
			_tabs[item] = null;
		}
		CurrentTab?.HideTab();
		_content.Clear();
		CurrentTab = null;
		_entities.Clear();
		_eventBus.Unregister(this);
		_batchControlBoxTimeRangeController.SetTimeRangeDropdownProvider(null);
	}

	[OnEvent]
	public void OnEntityDeleted(EntityDeletedEvent entityDeletedEvent)
	{
		EntityComponent entity = entityDeletedEvent.Entity;
		_entities.Remove(entity);
		foreach (BatchControlTab tab in Tabs)
		{
			tab.RemoveEntityRows(entity);
		}
		CurrentTab?.UpdateRowsVisibility();
	}

	private void AddTabs()
	{
		Dictionary<int, BatchControlTab> dictionary = new Dictionary<int, BatchControlTab>();
		foreach (BatchControlModule batchControlModule in _batchControlModules)
		{
			foreach (var (key, value) in batchControlModule.Tabs)
			{
				dictionary.Add(key, value);
			}
		}
		int num2 = 0;
		foreach (int item in dictionary.Keys.OrderBy((int result) => result))
		{
			BatchControlTab batchControlTab2 = dictionary[item];
			_tabs.Add(batchControlTab2, null);
			AddTabButton(batchControlTab2, num2++);
		}
	}

	private void AddTabButton(BatchControlTab batchControlTab, int tabIndex)
	{
		string elementName = "Game/BatchControl/BatchControlTabButton";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		Button button = visualElement.Q<Button>("BatchControlTabButton");
		button.RegisterCallback<ClickEvent>(delegate
		{
			ShowTab(tabIndex);
		});
		_tooltipRegistrar.Register(button, _loc.T(batchControlTab.TabNameLocKey));
		string path = Path.Combine(SpriteDirectory, batchControlTab.TabImage);
		Sprite sprite = _assetLoader.Load<Sprite>(path);
		visualElement.Q<Image>("BatchControlTabImage").sprite = sprite;
		_tabButtons.Add(visualElement);
	}

	private void SetNewTab(BatchControlTab batchControlTab)
	{
		if (CurrentTab == null)
		{
			_eventBus.Register(this);
		}
		CurrentTab?.HideTab();
		_middleRow.ToggleDisplayStyle(batchControlTab.MiddleRowVisible);
		_header.text = _loc.T(batchControlTab.TabNameLocKey);
		_batchControlBoxTimeRangeController.SetTimeRangeDropdownProvider(batchControlTab.TimeRangeDropdownProvider);
		_content.Clear();
		_content.Add(GetTabElement(batchControlTab));
		CurrentTab?.ShowTab();
	}

	private void UpdateActiveButtonClass(int index)
	{
		foreach (VisualElement item in _tabButtons.Children())
		{
			item.RemoveFromClassList(ActiveButtonClass);
		}
		_tabButtons[index].AddToClassList(ActiveButtonClass);
	}

	private VisualElement GetTabElement(BatchControlTab batchControlTab)
	{
		VisualElement visualElement = _tabs[batchControlTab] ?? batchControlTab.GetContent(_entities);
		_tabs[batchControlTab] = visualElement;
		CurrentTab = batchControlTab;
		return visualElement;
	}

	private void Refresh()
	{
		CurrentTab.Clear();
		_tabs[CurrentTab] = null;
		_content.Clear();
		_content.Add(GetTabElement(CurrentTab));
		CurrentTab.UpdateRowsVisibility();
		CurrentTab.IsDirty = false;
	}
}
