namespace Timberborn.ToolSystem;

public class TemporaryToolEnteredEvent
{
	public ITool Tool { get; }

	public TemporaryToolEnteredEvent(ITool tool)
	{
		Tool = tool;
	}
}
