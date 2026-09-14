using System;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Coordinates;
using Timberborn.Goods;
using UnityEngine;

namespace Timberborn.StockpileVisualization;

public class StockpilePlaneVisualizer : BaseComponent, IAwakableComponent, IStockpileVisualizer
{
	private readonly GoodVisualizationSpecService _goodVisualizationSpecService;

	private BlockObject _blockObject;

	private BlockObjectCenter _blockObjectCenter;

	private GoodVisualization _goodVisualization;

	private StockpilePlaneVisualizerSpec _stockpilePlaneVisualizerSpec;

	private GoodVisualizationSpec _currentVisualization;

	private StockpilePlaneVisualization _currentPlaneVisualization;

	private int _maxGoodAmount;

	private ImmutableArray<StockpilePlaneVisualization> Visualizations => _stockpilePlaneVisualizerSpec.StockpilePlaneVisualizations;

	public StockpilePlaneVisualizer(GoodVisualizationSpecService goodVisualizationSpecService)
	{
		_goodVisualizationSpecService = goodVisualizationSpecService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_blockObjectCenter = GetComponent<BlockObjectCenter>();
		_goodVisualization = GetComponent<GoodVisualization>();
		_stockpilePlaneVisualizerSpec = GetComponent<StockpilePlaneVisualizerSpec>();
	}

	public bool CanVisualize(string stockpileVisualization)
	{
		return Visualizations.Any((StockpilePlaneVisualization v) => v.GoodVisualizationId == stockpileVisualization);
	}

	public void Initialize(GoodSpec goodSpec, int capacity)
	{
		_currentPlaneVisualization = GetPlaneVisualization(goodSpec.StockpileVisualization);
		_currentVisualization = _goodVisualizationSpecService.GetVisualization(_currentPlaneVisualization.GoodVisualizationId, _currentPlaneVisualization.GoodVisualizationVariant);
		_maxGoodAmount = capacity;
		_goodVisualization.SetLocalPosition(CoordinateSystem.GridToWorld(_currentPlaneVisualization.CenterOffset));
		SetMaterial(goodSpec);
		_goodVisualization.SetMesh(_currentVisualization.PrimaryMesh.Asset);
	}

	public void UpdateAmount(int amountInStock)
	{
		float num = Mathf.Clamp01((float)amountInStock / (float)_maxGoodAmount);
		float nonLinearity = _currentVisualization.NonLinearity;
		if (nonLinearity != 0f)
		{
			num = (float)Math.Pow(num, nonLinearity + 1f);
		}
		SetTargetHeight(num);
	}

	public void Clear()
	{
		_currentVisualization = null;
		_maxGoodAmount = 0;
		_goodVisualization.Clear();
	}

	private StockpilePlaneVisualization GetPlaneVisualization(string stockpileVisualization)
	{
		return Visualizations.Single((StockpilePlaneVisualization v) => v.GoodVisualizationId == stockpileVisualization);
	}

	private void SetMaterial(GoodSpec goodSpec)
	{
		AssetRef<Material> assetRef = goodSpec.ContainerMaterial ?? _currentVisualization.Material;
		_goodVisualization.SetMaterial(assetRef.Asset, _currentPlaneVisualization.CenterOffset.z);
	}

	private void SetTargetHeight(float inventoryFillProgress)
	{
		Vector2 movementRange = _currentPlaneVisualization.MovementRange;
		float b = ((_currentVisualization.LimitingAmount != 0f) ? Math.Min(movementRange.y, _currentVisualization.LimitingAmount) : movementRange.y);
		float num = Mathf.Lerp(movementRange.x, b, inventoryFillProgress);
		Vector3 position = CoordinateSystem.GridToWorld(_blockObjectCenter.GridCenterGrounded + _currentPlaneVisualization.CenterOffset + _currentVisualization.Offset + Vector3.forward * num);
		Quaternion quaternion = _blockObject.Orientation.ToWorldSpaceRotation();
		_goodVisualization.SetPositionAndRotation(position, quaternion);
	}
}
