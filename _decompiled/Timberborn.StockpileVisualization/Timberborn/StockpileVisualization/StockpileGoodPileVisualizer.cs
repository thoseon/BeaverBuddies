using System;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.Goods;
using UnityEngine;

namespace Timberborn.StockpileVisualization;

internal class StockpileGoodPileVisualizer : BaseComponent, IAwakableComponent, IStockpileVisualizer
{
	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly GoodVisualizationSpecService _goodVisualizationSpecService;

	private readonly GoodPileVariantsService _goodPileVariantsService;

	private BlockObject _blockObject;

	private GoodVisualization _goodVisualization;

	private StockpileGoodPileVisualizerSpec _stockpileGoodPileVisualizerSpec;

	private int _maxNumberOfVisualizedGoods;

	private bool _rotated;

	private int _perLevelAmount;

	public GoodVisualizationSpec CurrentVisualization { get; private set; }

	public StockpileGoodPileVisualizer(IRandomNumberGenerator randomNumberGenerator, GoodVisualizationSpecService goodVisualizationSpecService, GoodPileVariantsService goodPileVariantsService)
	{
		_randomNumberGenerator = randomNumberGenerator;
		_goodVisualizationSpecService = goodVisualizationSpecService;
		_goodPileVariantsService = goodPileVariantsService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_goodVisualization = GetComponent<GoodVisualization>();
		_stockpileGoodPileVisualizerSpec = GetComponent<StockpileGoodPileVisualizerSpec>();
		_rotated = _randomNumberGenerator.CheckProbability(0.5f);
	}

	public bool CanVisualize(string stockpileVisualization)
	{
		ImmutableArray<string> goodPileVisualizations = _stockpileGoodPileVisualizerSpec.GoodPileVisualizations;
		for (int i = 0; i < goodPileVisualizations.Length; i++)
		{
			if (goodPileVisualizations[i] == stockpileVisualization)
			{
				return true;
			}
		}
		return false;
	}

	public void Initialize(GoodSpec goodSpec, int capacity)
	{
		CurrentVisualization = _goodVisualizationSpecService.GetVisualization(goodSpec.StockpileVisualization);
		CalculateAmounts(capacity);
		_goodVisualization.SetMaterial(CurrentVisualization.Material.Asset, _stockpileGoodPileVisualizerSpec.CenterOffset.z);
	}

	public void UpdateAmount(int amountInStock)
	{
		int num = Math.Min(amountInStock, _maxNumberOfVisualizedGoods);
		int num2 = num / _perLevelAmount;
		float y = (float)num2 * CurrentVisualization.Offset.z - 0.01f;
		Vector3 localPosition = CoordinateSystem.GridToWorld(_stockpileGoodPileVisualizerSpec.CenterOffset) + new Vector3(0f, y);
		_goodVisualization.SetLocalPosition(localPosition);
		int amount = num % _perLevelAmount;
		bool rotated = (_rotated ? (num2 % 2 == 0) : (num2 % 2 != 0));
		Mesh variant = _goodPileVariantsService.GetVariant(this, amount, rotated);
		_goodVisualization.SetMesh(variant);
	}

	public void Clear()
	{
		CurrentVisualization = null;
		_perLevelAmount = 0;
		_maxNumberOfVisualizedGoods = 0;
		_goodVisualization.Clear();
	}

	private void CalculateAmounts(int capacity)
	{
		int num = _blockObject.Blocks.GetOccupiedCoordinates().Count((Vector3Int coords) => coords.z == _blockObject.BaseZ);
		_perLevelAmount = CurrentVisualization.LimitingAmountFlooredToInt * num;
		_maxNumberOfVisualizedGoods = capacity;
	}
}
