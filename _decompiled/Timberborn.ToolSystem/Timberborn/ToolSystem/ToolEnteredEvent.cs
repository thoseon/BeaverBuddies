namespace Timberborn.ToolSystem;

public class ToolEnteredEvent
{
	public ITool Tool { get; }

	public bool ShouldCloseGroup { get; }

	public ToolEnteredEvent(ITool tool, bool shouldCloseGroup)
	{
		Tool = tool;
		ShouldCloseGroup = shouldCloseGroup;
	}
}
