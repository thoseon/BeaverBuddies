using UnityEngine;

namespace Timberborn.BlockSystem;

public readonly struct Block
{
	public Vector3Int Coordinates { get; }

	public MatterBelow MatterBelow { get; }

	public BlockOccupations Occupation { get; }

	public BlockStackable Stackable { get; }

	public bool OccupyAllBelow { get; }

	public bool OptionallyUnderground { get; }

	public bool Underground { get; }

	public bool IsOccupied => Occupation != BlockOccupations.None;

	public bool IsFoundationBlock => MatterBelow.IsSolidMatter();

	private Block(Vector3Int coordinates, MatterBelow matterBelow, BlockOccupations occupation, BlockStackable stackable, bool occupyAllBelow, bool optionallyUnderground, bool underground)
	{
		Coordinates = coordinates;
		MatterBelow = matterBelow;
		Occupation = occupation;
		Stackable = stackable;
		OccupyAllBelow = occupyAllBelow;
		OptionallyUnderground = optionallyUnderground;
		Underground = underground;
	}

	public static Block From(Vector3Int coordinates, BlockSpec blockSpec)
	{
		return new Block(coordinates, blockSpec.MatterBelow, blockSpec.Occupations, blockSpec.Stackable, blockSpec.OccupyAllBelow, optionallyUnderground: false, blockSpec.Underground);
	}

	public static Block From(Vector3Int coordinates, Block block)
	{
		return new Block(coordinates, block.MatterBelow, block.Occupation, block.Stackable, block.OccupyAllBelow, block.OptionallyUnderground, block.Underground);
	}

	public static Block FullFrom(Vector3Int coordinates)
	{
		return new Block(coordinates, MatterBelow.Any, BlockOccupations.All, BlockStackable.None, occupyAllBelow: false, optionallyUnderground: true, underground: false);
	}

	public bool IsIntersecting(Block other)
	{
		if (Coordinates == other.Coordinates)
		{
			return Occupation.Intersects(other.Occupation);
		}
		return false;
	}
}
