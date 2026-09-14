namespace Timberborn.BlockSystem;

public class BlockObjectUnsetEvent
{
	public BlockObject BlockObject { get; }

	public BlockObjectUnsetEvent(BlockObject blockObject)
	{
		BlockObject = blockObject;
	}
}
