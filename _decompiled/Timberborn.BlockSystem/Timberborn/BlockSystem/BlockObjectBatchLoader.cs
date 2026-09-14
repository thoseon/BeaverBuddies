using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.EntitySystem;

namespace Timberborn.BlockSystem;

public class BlockObjectBatchLoader
{
	public void AddToServices(IEnumerable<EntityComponent> entities)
	{
		foreach (BlockObject item in (from component in entities
			select component.GetComponent<BlockObject>() into blockObject
			where blockObject
			orderby blockObject.CoordinatesAtBaseZ.z
			select blockObject).ThenBy(GetHighestOccupation).ToList())
		{
			item.AddToServiceAfterLoad();
		}
	}

	private static int GetHighestOccupation(BlockObject blockObject)
	{
		int num = 0;
		foreach (Block allBlock in blockObject.Blocks.GetAllBlocks())
		{
			if (allBlock.Coordinates.z == blockObject.BaseZ)
			{
				int blockHighestOccupation = GetBlockHighestOccupation(allBlock);
				if (blockHighestOccupation > num)
				{
					num = blockHighestOccupation;
				}
			}
		}
		return num;
	}

	private static int GetBlockHighestOccupation(Block block)
	{
		BlockOccupations occupation = block.Occupation;
		if (occupation == BlockOccupations.None)
		{
			return 0;
		}
		if ((occupation & BlockOccupations.Top) != BlockOccupations.None)
		{
			return 6;
		}
		if ((occupation & BlockOccupations.Corners) != BlockOccupations.None)
		{
			return 5;
		}
		if ((occupation & BlockOccupations.Middle) != BlockOccupations.None)
		{
			return 4;
		}
		if ((occupation & BlockOccupations.Bottom) != BlockOccupations.None)
		{
			return 3;
		}
		if ((occupation & BlockOccupations.Path) != BlockOccupations.None)
		{
			return 2;
		}
		if ((occupation & BlockOccupations.Floor) != BlockOccupations.None)
		{
			return 1;
		}
		throw new ArgumentOutOfRangeException("occupation", occupation, "Unknown occupation type");
	}
}
