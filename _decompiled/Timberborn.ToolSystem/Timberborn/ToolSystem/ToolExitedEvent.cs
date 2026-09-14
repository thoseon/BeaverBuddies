namespace Timberborn.ToolSystem;

public class ToolExitedEvent
{
	public ITool Tool { get; }

	public ToolExitedEvent(ITool tool)
	{
		Tool = tool;
	}
}
