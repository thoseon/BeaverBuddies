using System;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.Goods;
using UnityEngine;

namespace Timberborn.StockpileVisualization;

public class StockpileGoodColumnVisualizer : BaseComponent, IAwakableComponent, IStockpileVisualizer
{
	private readonly GoodVisualizationSpecService _goodVisualizationSpecService;

	private readonly GoodColumnVariantsService _goodColumnVariantsService;

	private BlockObject _blockObject;

	private GoodVisualization _goodVisualization;

	private StockpileGoodColumnVisualizerSpec _stockpileGoodColumnVisualizerSpec;

	private GoodSpec _goodSpec;

	private int _perLevelAmount;

	private float _capacityFactor;

	private int _maxNumberOfVisualizedGoods;

	public GoodVisualizationSpec CurrentVisualization { get; private set; }

	private string GoodVisualizationId => _stockpileGoodColumnVisualizerSpec.GoodVisualizationId;

	public StockpileGoodColumnVisualizer(GoodVisualizationSpecService goodVisualizationSpecService, GoodColumnVariantsService goodColumnVariantsService)
	{
		_goodVisualizationSpecService = goodVisualizationSpecService;
		_goodColumnVariantsService = goodColumnVariantsService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_goodVisualization = GetComponent<GoodVisualization>();
		_stockpileGoodColumnVisualizerSpec = GetComponent<StockpileGoodColumnVisualizerSpec>();
	}

	public bool CanVisualize(string stockpileVisualization)
	{
		return GoodVisualizationId == stockpileVisualization;
	}

	public void Initialize(GoodSpec goodSpec, int capacity)
	{
		CurrentVisualization = _goodVisualizationSpecService.GetVisualization(GoodVisualizationId, _stockpileGoodColumnVisualizerSpec.GoodVisualizationVariant);
		_goodSpec = goodSpec;
		CalculateAmounts(capacity);
		_goodVisualization.SetMaterial(CurrentVisualization.Material.Asset, _stockpileGoodColumnVisualizerSpec.CenterOffset.z);
		_goodVisualization.SetIcon(goodSpec);
	}

	public void UpdateAmount(int amountInStock)
	{
		int num = Math.Min(Mathf.CeilToInt((float)amountInStock * _capacityFactor), _maxNumberOfVisualizedGoods);
		float y = (float)(num / _perLevelAmount) * CurrentVisualization.Offset.z;
		Vector3 localPosition = CoordinateSystem.GridToWorld(_stockpileGoodColumnVisualizerSpec.CenterOffset) + new Vector3(0f, y);
		_goodVisualization.SetLocalPosition(localPosition);
		int amount = num % _perLevelAmount;
		Mesh variant = _goodColumnVariantsService.GetVariant(this, amount);
		_goodVisualization.SetMesh(variant);
	}

	public void Clear()
	{
		CurrentVisualization = null;
		_goodSpec = null;
		_perLevelAmount = 0;
		_maxNumberOfVisualizedGoods = 0;
		_capacityFactor = 0f;
		_goodVisualization.Clear();
	}

	public void OverrideColor(Color color)
	{
		_goodVisualization.SetMaterial(CurrentVisualization.Material.Asset, _stockpileGoodColumnVisualizerSpec.CenterOffset.z);
		_goodVisualization.SetIcon(_goodSpec, color);
	}

	private void CalculateAmounts(int capacity)
	{
		int num = _blockObject.Blocks.GetOccupiedCoordinates().Count((Vector3Int coords) => coords.z == _blockObject.BaseZ);
		_perLevelAmount = 9 * num;
		_maxNumberOfVisualizedGoods = _perLevelAmount * CurrentVisualization.LimitingAmountFlooredToInt;
		_capacityFactor = (float)_maxNumberOfVisualizedGoods / (float)capacity;
	}
}
