namespace Timberborn.ToolSystem;

public class ToolUnlockedEvent
{
	public ITool Tool { get; }

	public ToolUnlockedEvent(ITool tool)
	{
		Tool = tool;
	}
}
