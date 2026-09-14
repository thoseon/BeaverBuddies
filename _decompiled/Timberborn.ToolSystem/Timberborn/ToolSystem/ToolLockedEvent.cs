namespace Timberborn.ToolSystem;

public class ToolLockedEvent
{
	public ITool Tool { get; }

	public ToolLockedEvent(ITool tool)
	{
		Tool = tool;
	}
}
