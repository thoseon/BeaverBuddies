using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.GoodConsumingBuildingSystem;
using Timberborn.MechanicalSystem;
using Timberborn.TickSystem;

namespace Timberborn.PowerGeneration;

public class GoodPoweredGenerator : TickableComponent, IAwakableComponent, IFinishedStateListener
{
	private MechanicalNode _mechanicalNode;

	private GoodConsumingBuilding _goodConsumingBuilding;

	private GoodConsumingToggle _goodConsumingToggle;

	public void Awake()
	{
		_mechanicalNode = GetComponent<MechanicalNode>();
		_goodConsumingBuilding = GetComponent<GoodConsumingBuilding>();
		_goodConsumingToggle = _goodConsumingBuilding.GetGoodConsumingToggle();
		_mechanicalNode.AddedToGraph += OnAddedToGraph;
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		UpdateState();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	public override void Tick()
	{
		UpdateState();
	}

	private void OnAddedToGraph(object sender, EventArgs e)
	{
		UpdateState();
	}

	private void UpdateState()
	{
		if (_mechanicalNode.Graph == null || (!_mechanicalNode.Graph.RequiresPower && !_goodConsumingBuilding.ConsumptionPaused))
		{
			_goodConsumingToggle.PauseConsumption();
		}
		else if (_mechanicalNode.Graph.RequiresPower && _goodConsumingBuilding.ConsumptionPaused)
		{
			_goodConsumingToggle.ResumeConsumption();
		}
		_mechanicalNode.SetOutputMultiplier(_goodConsumingBuilding.IsConsuming ? 1f : 0f);
	}
}
