using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.BlockSystem;

public record BlockObjectSpec : ComponentSpec
{
	[Serialize]
	public Vector3Int Size { get; init; }

	[Serialize]
	public ImmutableArray<BlockSpec> Blocks { get; init; }

	[Serialize]
	public EntranceBlockSpec Entrance { get; init; }

	[Serialize]
	public int BaseZ { get; init; }

	[Serialize]
	public bool Overridable { get; init; }

	[Serialize]
	public bool Flippable { get; init; }

	public Blocks GetBlocks()
	{
		return Timberborn.BlockSystem.Blocks.From(this);
	}

	public IEnumerable<Block> GetBlocks(Placement placement)
	{
		int x = 0;
		while (x < Size.x)
		{
			int num;
			for (int y = 0; y < Size.y; y = num)
			{
				for (int z = 0; z < Size.z; z = num)
				{
					Vector3Int coordinates = new Vector3Int(x, y, z);
					BlockSpec blockSpec = BlockSpecFromCoordinates(coordinates);
					Vector3Int coordinates2 = placement.Orientation.Transform(placement.FlipMode.Transform(coordinates, Size.x)) + placement.Coordinates;
					yield return Block.From(coordinates2, blockSpec);
					num = z + 1;
				}
				num = y + 1;
			}
			num = x + 1;
			x = num;
		}
	}

	public BlockSpec BlockSpecFromCoordinates(Vector3Int coordinates)
	{
		int index = (coordinates.z * Size.y + coordinates.y) * Size.x + coordinates.x;
		return Blocks[index];
	}

	public Vector3 CalculateCenterOffset(Orientation orientation)
	{
		return orientation.Transform(new Vector3(Size.x, Size.y, Size.z) / 2f);
	}
}
