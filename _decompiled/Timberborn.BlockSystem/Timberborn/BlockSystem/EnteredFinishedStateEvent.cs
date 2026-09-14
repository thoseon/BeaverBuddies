namespace Timberborn.BlockSystem;

public class EnteredFinishedStateEvent
{
	public BlockObject BlockObject { get; }

	public EnteredFinishedStateEvent(BlockObject blockObject)
	{
		BlockObject = blockObject;
	}
}
