using Timberborn.SimulationSystem;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;

namespace Timberborn.MapEditorSimulationSystem;

public class MapEditorSimulation : ILoadableSingleton
{
	private readonly SimulationController _simulationController;

	private readonly SpeedManager _speedManager;

	public int SimulationSpeed { get; private set; }

	public MapEditorSimulation(SimulationController simulationController, SpeedManager speedManager)
	{
		_simulationController = simulationController;
		_speedManager = speedManager;
	}

	public void Load()
	{
		SetSimulationSpeed(0);
	}

	public void SetSimulationSpeed(int simulationSpeed)
	{
		SimulationSpeed = simulationSpeed;
		_speedManager.ChangeSpeed(simulationSpeed);
	}

	public void ResetSimulation()
	{
		_simulationController.ResetSimulation();
	}
}
