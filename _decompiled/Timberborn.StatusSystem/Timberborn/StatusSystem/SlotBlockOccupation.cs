using Timberborn.BlockSystem;
using UnityEngine;

namespace Timberborn.StatusSystem;

internal static class SlotBlockOccupation
{
	public static readonly BlockOccupations Default = ~(BlockOccupations.Top | BlockOccupations.Corners);

	private static readonly BlockOccupations TileCorner = BlockOccupations.All;

	private static readonly BlockOccupations Middle = ~BlockOccupations.Corners;

	public static BlockOccupations GetOccupation(Vector2Int key, bool isMiddleSlot)
	{
		if (key.x % 2 == 0 && key.y % 2 == 0)
		{
			return TileCorner;
		}
		if (!isMiddleSlot)
		{
			return Default;
		}
		return Middle;
	}
}
