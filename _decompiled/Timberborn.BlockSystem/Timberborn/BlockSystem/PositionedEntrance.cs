using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.BlockSystem;

public class PositionedEntrance
{
	private static readonly Direction2D OnlyAllowedEntranceDirection;

	public Vector3Int Coordinates { get; }

	public Direction2D Direction2D { get; }

	public Vector3Int DoorstepCoordinates => Coordinates - Direction2D.ToOffset();

	private PositionedEntrance(Vector3Int coordinates, Direction2D direction2D)
	{
		Coordinates = coordinates;
		Direction2D = direction2D;
	}

	public static PositionedEntrance From(Blocks blocks, EntranceBlockSpec spec, Placement placement)
	{
		if (spec.HasEntrance)
		{
			Vector3Int coordinates = spec.Coordinates;
			Direction2D direction2D = placement.Orientation.Transform(OnlyAllowedEntranceDirection);
			return new PositionedEntrance(blocks.Transform(coordinates, placement), direction2D);
		}
		return null;
	}
}
