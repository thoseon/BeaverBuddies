namespace Timberborn.ToolSystem;

public class ToolGroupExitedEvent
{
	public ToolGroupSpec ToolGroup { get; }

	public ToolGroupExitedEvent(ToolGroupSpec toolGroup)
	{
		ToolGroup = toolGroup;
	}
}
