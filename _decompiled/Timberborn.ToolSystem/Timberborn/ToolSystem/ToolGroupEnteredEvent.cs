namespace Timberborn.ToolSystem;

public class ToolGroupEnteredEvent
{
	public ToolGroupSpec ToolGroup { get; }

	public ToolGroupEnteredEvent(ToolGroupSpec toolGroup)
	{
		ToolGroup = toolGroup;
	}
}
