namespace Timberborn.MechanicalSystem;

public class MechanicalGraphGeneratorAddedEvent
{
	public MechanicalGraph MechanicalGraph { get; }

	public MechanicalGraphGeneratorAddedEvent(MechanicalGraph mechanicalGraph)
	{
		MechanicalGraph = mechanicalGraph;
	}
}
