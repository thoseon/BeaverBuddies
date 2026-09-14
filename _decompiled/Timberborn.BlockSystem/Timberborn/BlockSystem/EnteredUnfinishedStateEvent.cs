namespace Timberborn.BlockSystem;

public class EnteredUnfinishedStateEvent
{
	public BlockObject BlockObject { get; }

	public EnteredUnfinishedStateEvent(BlockObject blockObject)
	{
		BlockObject = blockObject;
	}
}
