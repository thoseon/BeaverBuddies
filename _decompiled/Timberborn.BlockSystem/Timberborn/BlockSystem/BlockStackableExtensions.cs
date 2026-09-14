namespace Timberborn.BlockSystem;

public static class BlockStackableExtensions
{
	public static bool IsStackable(this BlockStackable blockStackable)
	{
		if (blockStackable != BlockStackable.BlockObject)
		{
			return blockStackable.IsUnfinishedGround();
		}
		return true;
	}

	public static bool IsUnfinishedGround(this BlockStackable blockStackable)
	{
		return blockStackable == BlockStackable.UnfinishedGround;
	}
}
