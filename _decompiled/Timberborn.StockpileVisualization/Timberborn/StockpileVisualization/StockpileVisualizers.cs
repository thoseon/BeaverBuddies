using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Persistence;
using Timberborn.SelectionSystem;
using Timberborn.Stockpiles;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.StockpileVisualization;

public class StockpileVisualizers : BaseComponent, IAwakableComponent, IFinishedStateListener, IPersistentEntity
{
	private static readonly ComponentKey StockpileVisualizersKey = new ComponentKey("StockpileVisualizers");

	private static readonly PropertyKey<SerializedGood> CurrentGoodKey = new PropertyKey<SerializedGood>("CurrentGood");

	private readonly IGoodService _goodService;

	private readonly SerializedGoodValueSerializer _serializedGoodValueSerializer;

	private readonly List<IStockpileVisualizer> _visualizers = new List<IStockpileVisualizer>();

	private Inventory _inventory;

	private SingleGoodAllower _singleGoodAllower;

	private HighlightableObject _highlightableObject;

	private IStockpileVisualizer _currentVisualizer;

	private string _currentGoodId;

	private string _awaitingGoodId;

	public StockpileVisualizers(IGoodService goodService, SerializedGoodValueSerializer serializedGoodValueSerializer)
	{
		_goodService = goodService;
		_serializedGoodValueSerializer = serializedGoodValueSerializer;
	}

	public void Awake()
	{
		GetComponents(_visualizers);
		_inventory = GetComponent<Stockpile>().Inventory;
		_singleGoodAllower = GetComponent<SingleGoodAllower>();
		_highlightableObject = GetComponent<HighlightableObject>();
	}

	public void OnEnterFinishedState()
	{
		_inventory.InventoryChanged += OnInventoryChanged;
		_singleGoodAllower.DisallowedGoodsChanged += OnDisallowedGoodsChanged;
		if (_singleGoodAllower.HasAllowedGood)
		{
			SetAwaitingOrCurrentVisualizer(_singleGoodAllower.AllowedGood);
		}
		if (_currentGoodId != null)
		{
			SetCurrentVisualizer(_currentGoodId);
		}
	}

	public void OnExitFinishedState()
	{
		_inventory.InventoryChanged -= OnInventoryChanged;
		_singleGoodAllower.DisallowedGoodsChanged -= OnDisallowedGoodsChanged;
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_currentGoodId != null)
		{
			entitySaver.GetComponent(StockpileVisualizersKey).Set(CurrentGoodKey, new SerializedGood(_currentGoodId), _serializedGoodValueSerializer);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(StockpileVisualizersKey, out var objectLoader) && objectLoader.GetObsoletable(CurrentGoodKey, _serializedGoodValueSerializer, out var value))
		{
			_currentGoodId = value.Id;
		}
	}

	public void SetCurrentVisualizer(string goodId)
	{
		_currentGoodId = goodId;
		GoodSpec good = _goodService.GetGood(goodId);
		_currentVisualizer?.Clear();
		_currentVisualizer = GetVisualizer(good);
		if (_currentVisualizer != null)
		{
			_currentVisualizer.Initialize(good, _inventory.Capacity);
			_currentVisualizer.UpdateAmount(_inventory.TotalAmountInStock);
		}
		_highlightableObject.UpdateColorAndHighlight();
	}

	private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
	{
		_currentVisualizer?.UpdateAmount(_inventory.TotalAmountInStock);
		if (!string.IsNullOrEmpty(_awaitingGoodId) && _inventory.UnwantedStockAmount() == 0)
		{
			ResetAwaitingAndSetCurrentVisualizer(_awaitingGoodId);
		}
	}

	private void OnDisallowedGoodsChanged(object sender, DisallowedGoodsChangedEventArgs e)
	{
		SetAwaitingOrCurrentVisualizer(e.GoodId);
	}

	private void SetAwaitingOrCurrentVisualizer(string goodId)
	{
		if (_inventory.UnwantedStockAmount() > 0)
		{
			_awaitingGoodId = goodId;
		}
		else
		{
			ResetAwaitingAndSetCurrentVisualizer(goodId);
		}
	}

	private void ResetAwaitingAndSetCurrentVisualizer(string goodId)
	{
		_awaitingGoodId = null;
		if (_currentGoodId != goodId)
		{
			SetCurrentVisualizer(goodId);
		}
	}

	private IStockpileVisualizer GetVisualizer(GoodSpec goodSpec)
	{
		foreach (IStockpileVisualizer visualizer in _visualizers)
		{
			if (visualizer.CanVisualize(goodSpec.StockpileVisualization))
			{
				return visualizer;
			}
		}
		Debug.LogWarning("Unable to visualize " + goodSpec.Id + " in " + base.Name);
		return null;
	}
}
