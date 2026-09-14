using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockObstacles;
using Timberborn.BlockSystem;
using Timberborn.MechanicalSystem;
using UnityEngine;

namespace Timberborn.PowerManagement;

public class GravityBattery : BaseComponent, IAwakableComponent, IFinishedStateListener, IBattery
{
	private MechanicalNode _mechanicalNode;

	private LayeredBlockObstacle _layeredBlockObstacle;

	private GravityBatterySpec _gravityBatterySpec;

	public int CapacityPerTile => _gravityBatterySpec.CapacityPerTile;

	public void Awake()
	{
		_mechanicalNode = GetComponent<MechanicalNode>();
		_layeredBlockObstacle = GetComponent<LayeredBlockObstacle>();
		_gravityBatterySpec = GetComponent<GravityBatterySpec>();
	}

	public void OnEnterFinishedState()
	{
		UpdateNode();
		_layeredBlockObstacle.MaxOccupancyRangeChanged += OnDependenciesChanged;
	}

	public void OnExitFinishedState()
	{
	}

	public void ModifyCharge(float chargeDelta)
	{
		float occupancyRangeDelta = (0f - chargeDelta) / (float)CapacityPerTile;
		_layeredBlockObstacle.ModifyOccupancyRange(occupancyRangeDelta);
		UpdateNode();
	}

	private void OnDependenciesChanged(object sender, EventArgs e)
	{
		UpdateNode();
	}

	private void UpdateNode()
	{
		_mechanicalNode.SetNominalBatteryCharge(Mathf.CeilToInt((float)CapacityPerTile * (_layeredBlockObstacle.MaxOccupancyRange - _layeredBlockObstacle.OccupancyRange)));
		_mechanicalNode.SetNominalBatteryCapacity(Mathf.CeilToInt(_layeredBlockObstacle.MaxOccupancyRange * (float)CapacityPerTile));
	}
}
