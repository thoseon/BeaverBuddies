using Timberborn.TickSystem;

namespace Timberborn.MechanicalSystem;

internal class BatteryService : ITickableSingleton
{
	private readonly MechanicalGraphRegistry _mechanicalGraphRegistry;

	private readonly BatteryCharger _batteryCharger;

	private readonly BatteryDischarger _batteryDischarger;

	public BatteryService(MechanicalGraphRegistry mechanicalGraphRegistry, BatteryCharger batteryCharger, BatteryDischarger batteryDischarger)
	{
		_mechanicalGraphRegistry = mechanicalGraphRegistry;
		_batteryCharger = batteryCharger;
		_batteryDischarger = batteryDischarger;
	}

	public void Tick()
	{
		ChargeAndDischarge();
	}

	private void ChargeAndDischarge()
	{
		foreach (MechanicalGraph mechanicalGraph in _mechanicalGraphRegistry.MechanicalGraphs)
		{
			ChargeAndDischarge(mechanicalGraph);
		}
	}

	private void ChargeAndDischarge(MechanicalGraph mechanicalGraph)
	{
		int powerSurplus = mechanicalGraph.PowerSurplus;
		if (powerSurplus > 0)
		{
			_batteryCharger.Charge(mechanicalGraph, powerSurplus);
		}
		else if (powerSurplus < 0)
		{
			_batteryDischarger.Discharge(mechanicalGraph, -powerSurplus);
		}
	}
}
