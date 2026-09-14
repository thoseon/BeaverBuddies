using Timberborn.Debugging;
using Timberborn.SimulationSystem;

namespace Timberborn.WaterSystemUI;

internal class WaterSystemDevModule : IDevModule
{
	private readonly SimulationController _simulationController;

	public WaterSystemDevModule(SimulationController simulationController)
	{
		_simulationController = simulationController;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Water simulation: Reset all", Reset)).Build();
	}

	private void Reset()
	{
		_simulationController.ResetSimulation();
	}
}
