using System.Collections.Generic;
using Timberborn.TimeSystem;

namespace Timberborn.MechanicalSystem;

internal class BatteryCharger
{
	private readonly IDayNightCycle _dayNightCycle;

	private readonly List<MechanicalNode> _batteries = new List<MechanicalNode>();

	public BatteryCharger(IDayNightCycle dayNightCycle)
	{
		_dayNightCycle = dayNightCycle;
	}

	public void Charge(MechanicalGraph mechanicalGraph, int chargingPower)
	{
		GetChargableBatteries(mechanicalGraph);
		if (_batteries.Count > 0)
		{
			ChargeBatteries(chargingPower);
			_batteries.Clear();
		}
	}

	private void GetChargableBatteries(MechanicalGraph mechanicalGraph)
	{
		foreach (MechanicalNode battery in mechanicalGraph.Batteries)
		{
			if (battery.Active && battery.Actuals.BatteryCharge < battery.Actuals.BatteryCapacity)
			{
				_batteries.Add(battery);
			}
		}
	}

	private void ChargeBatteries(int chargingPower)
	{
		float chargeDelta = _dayNightCycle.FixedDeltaTimeInHours * (float)chargingPower / (float)_batteries.Count;
		foreach (MechanicalNode battery in _batteries)
		{
			battery.Battery.ModifyCharge(chargeDelta);
		}
	}
}
