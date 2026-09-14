namespace Timberborn.BlockSystem;

public class ExitedUnfinishedStateEvent
{
	public BlockObject BlockObject { get; }

	public ExitedUnfinishedStateEvent(BlockObject blockObject)
	{
		BlockObject = blockObject;
	}
}
