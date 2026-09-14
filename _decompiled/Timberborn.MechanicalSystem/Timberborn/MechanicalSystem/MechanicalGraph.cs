using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;
using UnityEngine;

namespace Timberborn.MechanicalSystem;

public class MechanicalGraph
{
	private readonly EventBus _eventBus;

	private readonly MechanicalGraphRegistry _mechanicalGraphRegistry;

	private readonly IDayNightCycle _dayNightCycle;

	private readonly HashSet<MechanicalNode> _nodes = new HashSet<MechanicalNode>();

	private readonly List<MechanicalNode> _generators = new List<MechanicalNode>();

	private readonly List<MechanicalNode> _batteries = new List<MechanicalNode>();

	public int PowerSupply { get; private set; }

	public int PowerDemand { get; private set; }

	public int BatteryCharge { get; private set; }

	public int BatteryCapacity { get; private set; }

	public IEnumerable<MechanicalNode> Nodes => _nodes.AsReadOnlyEnumerable();

	public ReadOnlyList<MechanicalNode> Generators => _generators.AsReadOnlyList();

	public ReadOnlyList<MechanicalNode> Batteries => _batteries.AsReadOnlyList();

	public int NumberOfGenerators => _generators.Count;

	public bool Powered
	{
		get
		{
			if (PowerSupply <= 0)
			{
				return BatteryCharge > 0;
			}
			return true;
		}
	}

	public int PowerSurplus => PowerSupply - PowerDemand;

	public bool RequiresPower
	{
		get
		{
			if (PowerDemand <= 0)
			{
				return BatteryCharge < BatteryCapacity;
			}
			return true;
		}
	}

	public bool Valid => _nodes.FirstOrDefault()?.Graph == this;

	public float BatteryChargeLevel
	{
		get
		{
			if (BatteryCapacity <= 0)
			{
				return 0f;
			}
			return (float)BatteryCharge / (float)BatteryCapacity;
		}
	}

	public float PowerEfficiency
	{
		get
		{
			if (PowerDemand <= 0)
			{
				return (PowerSupply > 0) ? 1 : 0;
			}
			return Mathf.Min((float)(PowerSupply + BatteryPower) / (float)PowerDemand, 1f);
		}
	}

	private int BatteryPower
	{
		get
		{
			if (PowerSurplus >= 0)
			{
				return 0;
			}
			return Mathf.Min(-PowerSurplus, MaxBatteryPowerThisTick);
		}
	}

	private int MaxBatteryPowerThisTick => Mathf.CeilToInt((float)BatteryCharge / _dayNightCycle.FixedDeltaTimeInHours);

	public MechanicalGraph(EventBus eventBus, MechanicalGraphRegistry mechanicalGraphRegistry, IDayNightCycle dayNightCycle)
	{
		_eventBus = eventBus;
		_mechanicalGraphRegistry = mechanicalGraphRegistry;
		_dayNightCycle = dayNightCycle;
	}

	internal void AddNode(MechanicalNode mechanicalNode)
	{
		if (_nodes.Add(mechanicalNode))
		{
			mechanicalNode.Graph?.RemoveNode(mechanicalNode);
			mechanicalNode.Graph = this;
			AddNodeToCounters(mechanicalNode);
			_mechanicalGraphRegistry.AddGraph(mechanicalNode.Graph);
			if (mechanicalNode.IsGenerator)
			{
				_generators.Add(mechanicalNode);
				_eventBus.Post(new MechanicalGraphGeneratorAddedEvent(mechanicalNode.Graph));
			}
			if (mechanicalNode.IsBattery)
			{
				_batteries.Add(mechanicalNode);
			}
		}
	}

	internal void RemoveNode(MechanicalNode mechanicalNode)
	{
		if (_nodes.Remove(mechanicalNode))
		{
			RemoveNodeFromCounters(mechanicalNode);
			_mechanicalGraphRegistry.RemoveGraph(mechanicalNode.Graph);
			mechanicalNode.Graph = null;
			if (mechanicalNode.IsGenerator)
			{
				_generators.Remove(mechanicalNode);
			}
			if (mechanicalNode.IsBattery)
			{
				_batteries.Remove(mechanicalNode);
			}
		}
	}

	private void AddNodeToCounters(MechanicalNode mechanicalNode)
	{
		MechanicalNodeActuals actuals = mechanicalNode.Actuals;
		PowerSupply += actuals.PowerOutput;
		PowerDemand += actuals.PowerInput;
		BatteryCharge += actuals.BatteryCharge;
		BatteryCapacity += actuals.BatteryCapacity;
		actuals.PowerOutputChanged += OnNodePowerOutputChanged;
		actuals.PowerInputChanged += OnNodePowerInputChanged;
		actuals.BatteryChargeChanged += OnNodeBatteryChargeChanged;
		actuals.BatteryCapacityChanged += OnNodeBatteryCapacityChanged;
	}

	private void RemoveNodeFromCounters(MechanicalNode mechanicalNode)
	{
		MechanicalNodeActuals actuals = mechanicalNode.Actuals;
		PowerSupply -= actuals.PowerOutput;
		PowerDemand -= actuals.PowerInput;
		BatteryCharge -= actuals.BatteryCharge;
		BatteryCapacity -= actuals.BatteryCapacity;
		actuals.PowerOutputChanged -= OnNodePowerOutputChanged;
		actuals.PowerInputChanged -= OnNodePowerInputChanged;
		actuals.BatteryChargeChanged -= OnNodeBatteryChargeChanged;
		actuals.BatteryCapacityChanged -= OnNodeBatteryCapacityChanged;
	}

	private void OnNodePowerOutputChanged(object sender, ValueChangedEventArgs<int> e)
	{
		PowerSupply += e.NewValue - e.OldValue;
	}

	private void OnNodePowerInputChanged(object sender, ValueChangedEventArgs<int> e)
	{
		PowerDemand += e.NewValue - e.OldValue;
	}

	private void OnNodeBatteryChargeChanged(object sender, ValueChangedEventArgs<int> e)
	{
		BatteryCharge += e.NewValue - e.OldValue;
	}

	private void OnNodeBatteryCapacityChanged(object sender, ValueChangedEventArgs<int> e)
	{
		BatteryCapacity += e.NewValue - e.OldValue;
	}
}
