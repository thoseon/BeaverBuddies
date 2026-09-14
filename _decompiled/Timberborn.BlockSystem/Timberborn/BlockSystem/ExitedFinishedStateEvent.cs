namespace Timberborn.BlockSystem;

public class ExitedFinishedStateEvent
{
	public BlockObject BlockObject { get; }

	public ExitedFinishedStateEvent(BlockObject blockObject)
	{
		BlockObject = blockObject;
	}
}
