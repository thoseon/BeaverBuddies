namespace Timberborn.BeaverContaminationSystem;

public class ContaminableContaminationChangedEvent
{
	public Contaminable Contaminable { get; }

	public ContaminableContaminationChangedEvent(Contaminable contaminable)
	{
		Contaminable = contaminable;
	}
}
