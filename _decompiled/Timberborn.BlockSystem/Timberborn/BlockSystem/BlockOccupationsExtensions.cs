namespace Timberborn.BlockSystem;

public static class BlockOccupationsExtensions
{
	public static bool Intersects(this BlockOccupations a, BlockOccupations b)
	{
		return (a & b) != 0;
	}

	public static bool IsBottomOrFloorOrBoth(this BlockOccupations blockOccupations)
	{
		if (blockOccupations != BlockOccupations.Floor && blockOccupations != BlockOccupations.Bottom)
		{
			return blockOccupations == (BlockOccupations.Floor | BlockOccupations.Bottom);
		}
		return true;
	}

	public static bool IsTopOrCornersOrBoth(this BlockOccupations blockOccupations)
	{
		if (blockOccupations != BlockOccupations.Top && blockOccupations != BlockOccupations.Corners)
		{
			return blockOccupations == (BlockOccupations.Top | BlockOccupations.Corners);
		}
		return true;
	}

	public static bool HasBottomOrFloorOrFull(this BlockOccupations blockOccupations)
	{
		if (!blockOccupations.HasFlag(BlockOccupations.Floor) && !blockOccupations.HasFlag(BlockOccupations.Bottom))
		{
			return blockOccupations.IsFull();
		}
		return true;
	}

	public static bool IsFull(this BlockOccupations blockOccupations)
	{
		return blockOccupations == BlockOccupations.All;
	}
}
