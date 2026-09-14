namespace Timberborn.ConstructionMode;

public class ConstructionModeChangedEvent
{
	public bool InConstructionMode { get; }

	public ConstructionModeChangedEvent(bool inConstructionMode)
	{
		InConstructionMode = inConstructionMode;
	}
}
