using System.Collections.Generic;
using Timberborn.TimeSystem;
using UnityEngine;

namespace Timberborn.MechanicalSystem;

internal class BatteryDischarger
{
	private readonly float _minChargeToRemovePerBattery = 0.01f;

	private readonly IDayNightCycle _dayNightCycle;

	private readonly List<MechanicalNode> _batteries = new List<MechanicalNode>();

	public BatteryDischarger(IDayNightCycle dayNightCycle)
	{
		_dayNightCycle = dayNightCycle;
	}

	public void Discharge(MechanicalGraph mechanicalGraph, int dischargingPower)
	{
		if (dischargingPower > 0)
		{
			GetDischargableBatteries(mechanicalGraph);
			if (_batteries.Count > 0)
			{
				DischargeBatteries(dischargingPower);
				_batteries.Clear();
			}
		}
	}

	private void GetDischargableBatteries(MechanicalGraph graph)
	{
		foreach (MechanicalNode battery in graph.Batteries)
		{
			if (battery.Active && battery.Actuals.BatteryCharge > 0)
			{
				_batteries.Add(battery);
			}
		}
	}

	private void DischargeBatteries(int dischargingPower)
	{
		float chargeToRemovePerBattery = Mathf.Max(_dayNightCycle.FixedDeltaTimeInHours * (float)dischargingPower / (float)_batteries.Count, _minChargeToRemovePerBattery);
		RemoveChargeFromBatteries(chargeToRemovePerBattery);
	}

	private void RemoveChargeFromBatteries(float chargeToRemovePerBattery)
	{
		foreach (MechanicalNode battery in _batteries)
		{
			battery.Battery.ModifyCharge(0f - chargeToRemovePerBattery);
		}
	}
}
