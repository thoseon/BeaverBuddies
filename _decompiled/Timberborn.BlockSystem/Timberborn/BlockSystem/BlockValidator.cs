using System.Linq;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.BlockSystem;

public class BlockValidator
{
	private readonly IBlockService _blockService;

	private readonly MatterBelowValidator _matterBelowValidator;

	private readonly ITerrainService _terrainService;

	private readonly StackableBlockService _stackableBlockService;

	public BlockValidator(IBlockService blockService, MatterBelowValidator matterBelowValidator, ITerrainService terrainService, StackableBlockService stackableBlockService)
	{
		_blockService = blockService;
		_matterBelowValidator = matterBelowValidator;
		_terrainService = terrainService;
		_stackableBlockService = stackableBlockService;
	}

	public bool BlocksValid(PositionedBlocks positionedBlocks)
	{
		return positionedBlocks.GetAllBlocks().All((Block block) => BlockValid(block, almost: false, ignoreUnfinishedStackable: false));
	}

	public bool BlocksValid(BlockObjectSpec blockObjectSpec, Placement placement)
	{
		return blockObjectSpec.GetBlocks(placement).All((Block block) => BlockValid(block, almost: false, ignoreUnfinishedStackable: false));
	}

	public bool BlocksAlmostValid(PositionedBlocks positionedBlocks)
	{
		return (from block in positionedBlocks.GetAllBlocks()
			where block.MatterBelow.IsSolidMatter()
			select block).Any((Block block) => BlockValid(block, almost: true, ignoreUnfinishedStackable: false));
	}

	public bool BlockValidWithoutUnfinishedStackable(Block block)
	{
		return BlockValid(block, almost: false, ignoreUnfinishedStackable: true);
	}

	private bool IsOccupiedByBlockAbove(Vector3Int coordinates)
	{
		Vector3Int coords = coordinates + new Vector3Int(0, 0, 1);
		return BlockRequiresAirBelow(coords);
	}

	private bool BlockValid(Block block, bool almost, bool ignoreUnfinishedStackable)
	{
		if (!FitsInMap(block, almost))
		{
			return false;
		}
		if (BlockConflictsWithExistingObject(block))
		{
			return false;
		}
		if (BlockConflictsWithBlockAbove(block))
		{
			return false;
		}
		if (BlockConflictsWithBlocksBelow(block))
		{
			return false;
		}
		if (BlockConflictsWithTerrain(block))
		{
			return false;
		}
		if (ConflictsWithUndergroundBlockObject(block))
		{
			return false;
		}
		if (UndergroundBlockIsNotUnderground(block))
		{
			return false;
		}
		if (!almost && BlockConflictsWithMatterBelow(block, ignoreUnfinishedStackable))
		{
			return false;
		}
		return true;
	}

	private bool FitsInMap(Block block, bool almost)
	{
		if (block.IsOccupied)
		{
			if (!((block.OptionallyUnderground || block.Underground) | almost))
			{
				return _blockService.Contains(block.Coordinates);
			}
			return _blockService.Contains(block.Coordinates.XY());
		}
		return true;
	}

	private bool BlockConflictsWithExistingObject(Block block)
	{
		return _blockService.AnyNonOverridableObjectsAt(block.Coordinates, block.Occupation);
	}

	private bool BlockConflictsWithBlockAbove(Block block)
	{
		if (block.Occupation.Intersects(BlockOccupations.Top))
		{
			return IsOccupiedByBlockAbove(block.Coordinates);
		}
		return false;
	}

	private bool BlockConflictsWithBlocksBelow(Block block)
	{
		if (block.OccupyAllBelow)
		{
			return _blockService.AnyNonOverridableObjectBelow(block.Coordinates);
		}
		return false;
	}

	private bool BlockRequiresAirBelow(Vector3Int coords)
	{
		BlockObject bottomObjectAt = _blockService.GetBottomObjectAt(coords);
		if ((bool)bottomObjectAt && bottomObjectAt.PositionedBlocks.GetBlock(coords).MatterBelow == MatterBelow.Air)
		{
			return true;
		}
		return false;
	}

	private bool BlockConflictsWithTerrain(Block block)
	{
		if (block.IsOccupied && !block.OptionallyUnderground && !block.Underground)
		{
			return _terrainService.Underground(block.Coordinates);
		}
		return false;
	}

	private bool ConflictsWithUndergroundBlockObject(Block block)
	{
		if (block.Underground)
		{
			return _blockService.GetObjectsAt(block.Coordinates).Any((BlockObject blockObject) => blockObject.PositionedBlocks.GetBlock(block.Coordinates).Underground);
		}
		return false;
	}

	private bool UndergroundBlockIsNotUnderground(Block block)
	{
		if (block.Underground && !_terrainService.Underground(block.Coordinates))
		{
			return !_stackableBlockService.IsUnfinishedGroundBlockAt(block.Coordinates);
		}
		return false;
	}

	private bool BlockConflictsWithMatterBelow(Block block, bool ignoreUnfinishedStackable)
	{
		if (!ignoreUnfinishedStackable)
		{
			return !_matterBelowValidator.Validate(in block);
		}
		return !_matterBelowValidator.ValidateIgnoringUnfinishedStackable(in block);
	}
}
