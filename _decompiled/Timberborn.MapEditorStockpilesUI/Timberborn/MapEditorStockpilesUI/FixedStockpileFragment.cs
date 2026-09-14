using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.EntityUndoSystem;
using Timberborn.InventorySystem;
using Timberborn.SingletonSystem;
using Timberborn.StockpileVisualization;
using Timberborn.Stockpiles;
using Timberborn.UndoSystem;
using UnityEngine.UIElements;

namespace Timberborn.MapEditorStockpilesUI;

internal class FixedStockpileFragment : IEntityPanelFragment, ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly EntityChangeRecorderFactory _entityChangeRecorderFactory;

	private FixedStockpileDropdownProvider _fixedStockpileDropdownProvider;

	private FixedStockpileInventorySetter _fixedStockpileInventorySetter;

	private StockpileVisualizationUpdater _stockpileVisualizationUpdater;

	private Inventory _inventory;

	private VisualElement _root;

	private Dropdown _goods;

	private IntegerField _amount;

	public FixedStockpileFragment(EventBus eventBus, VisualElementLoader visualElementLoader, DropdownItemsSetter dropdownItemsSetter, EntityChangeRecorderFactory entityChangeRecorderFactory)
	{
		_eventBus = eventBus;
		_visualElementLoader = visualElementLoader;
		_dropdownItemsSetter = dropdownItemsSetter;
		_entityChangeRecorderFactory = entityChangeRecorderFactory;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "MapEditor/EntityPanel/FixedStockpileFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_goods = _root.Q<Dropdown>("Goods");
		_amount = _root.Q<IntegerField>("Amount");
		_amount.RegisterValueChangedCallback(OnGoodAmountChanged);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		Stockpile component = entity.GetComponent<Stockpile>();
		if (component != null)
		{
			_fixedStockpileDropdownProvider = component.GetComponent<FixedStockpileDropdownProvider>();
			_fixedStockpileInventorySetter = component.GetComponent<FixedStockpileInventorySetter>();
			_stockpileVisualizationUpdater = component.GetComponent<StockpileVisualizationUpdater>();
			_inventory = component.Inventory;
			_amount.SetValueWithoutNotify(_inventory.TotalAmountInStock);
			_amount.isDelayed = true;
			_dropdownItemsSetter.SetItems(_goods, _fixedStockpileDropdownProvider);
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void ClearFragment()
	{
		_inventory = null;
		_fixedStockpileDropdownProvider = null;
		_fixedStockpileInventorySetter = null;
		_stockpileVisualizationUpdater = null;
		_root.ToggleDisplayStyle(visible: false);
		_goods.ClearItems();
	}

	public void UpdateFragment()
	{
	}

	[OnEvent]
	public void OnUndoStateChanged(UndoStateChangedEvent undoStateChangedEvent)
	{
		if ((bool)_inventory)
		{
			_goods.UpdateSelectedValue();
			_amount.SetValueWithoutNotify(_inventory.TotalAmountInStock);
			_stockpileVisualizationUpdater.UpdateVisualization();
		}
	}

	private void OnGoodAmountChanged(ChangeEvent<int> changeEvent)
	{
		int amount = Math.Clamp(changeEvent.newValue, 0, _inventory.Capacity);
		using (_entityChangeRecorderFactory.CreateChangeRecorder(_fixedStockpileInventorySetter))
		{
			_fixedStockpileInventorySetter.SetAmount(amount);
		}
	}
}
