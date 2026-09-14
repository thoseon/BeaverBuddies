namespace Timberborn.BlockSystem;

public class BlockObjectSetEvent
{
	public BlockObject BlockObject { get; }

	public BlockObjectSetEvent(BlockObject blockObject)
	{
		BlockObject = blockObject;
	}
}
