using Timberborn.MapEditorTickSystem;
using Timberborn.TickSystem;

namespace Timberborn.SimulationSystem;

[MapEditorTickable]
public class SimulationController : ITickableSingleton
{
	private bool _clearNextTick;

	public bool ShouldResetSimulation { get; private set; }

	public void Tick()
	{
		if (_clearNextTick)
		{
			ShouldResetSimulation = false;
			_clearNextTick = false;
		}
		else if (ShouldResetSimulation)
		{
			_clearNextTick = true;
		}
	}

	public void ResetSimulation()
	{
		ShouldResetSimulation = true;
		_clearNextTick = false;
	}
}
