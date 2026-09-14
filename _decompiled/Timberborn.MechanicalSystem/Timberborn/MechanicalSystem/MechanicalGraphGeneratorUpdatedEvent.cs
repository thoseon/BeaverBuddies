namespace Timberborn.MechanicalSystem;

public class MechanicalGraphGeneratorUpdatedEvent
{
	public MechanicalGraph MechanicalGraph { get; }

	public MechanicalGraphGeneratorUpdatedEvent(MechanicalGraph mechanicalGraph)
	{
		MechanicalGraph = mechanicalGraph;
	}
}
