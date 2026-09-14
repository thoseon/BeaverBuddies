using System;
using Timberborn.Common;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.BlockSystem;

public class MatterBelowValidator
{
	private readonly IBlockService _blockService;

	private readonly ITerrainService _terrainService;

	private readonly StackableBlockService _stackableBlockService;

	public MatterBelowValidator(IBlockService blockService, ITerrainService terrainService, StackableBlockService stackableBlockService)
	{
		_blockService = blockService;
		_terrainService = terrainService;
		_stackableBlockService = stackableBlockService;
	}

	public bool Validate(in Block block)
	{
		return Validate(in block, ignoreUnfinishedStackable: false);
	}

	public bool ValidateIgnoringUnfinishedStackable(in Block block)
	{
		return Validate(in block, ignoreUnfinishedStackable: true);
	}

	private bool Validate(in Block block, bool ignoreUnfinishedStackable)
	{
		MatterBelow matterBelow = block.MatterBelow;
		Vector3Int coordinates = block.Coordinates;
		return matterBelow switch
		{
			MatterBelow.Any => true, 
			MatterBelow.Air => AboveGround(coordinates) && !TopBlockBelow(coordinates), 
			MatterBelow.Ground => AtGroundLevel(coordinates) || (!ignoreUnfinishedStackable && UnfinishedGroundBelow(coordinates)) || (block.Underground && _terrainService.Underground(coordinates.Below())), 
			MatterBelow.Stackable => StackableBelow(coordinates, ignoreUnfinishedStackable), 
			MatterBelow.GroundOrStackable => AtGroundLevel(coordinates) || (block.Underground && _terrainService.Underground(coordinates.Below())) || StackableBelow(coordinates, ignoreUnfinishedStackable), 
			_ => throw new NotSupportedException(matterBelow.ToString()), 
		};
	}

	private bool AtGroundLevel(Vector3Int coordinates)
	{
		if (!_terrainService.Underground(coordinates))
		{
			return _terrainService.Underground(coordinates.Below());
		}
		return false;
	}

	private bool AboveGround(Vector3Int coordinates)
	{
		if (!_terrainService.Underground(coordinates))
		{
			return !_terrainService.Underground(coordinates.Below());
		}
		return false;
	}

	private bool TopBlockBelow(Vector3Int coordinates)
	{
		return _blockService.AnyNonOverridableObjectsAt(coordinates.Below(), BlockOccupations.Top);
	}

	private bool StackableBelow(Vector3Int coordinates, bool ignoreUnfinishedStackable)
	{
		Vector3Int coords = coordinates - new Vector3Int(0, 0, 1);
		if (!ignoreUnfinishedStackable)
		{
			return _stackableBlockService.IsStackableBlockAt(coords);
		}
		return _stackableBlockService.IsFinishedStackableBlockAt(coords);
	}

	private bool UnfinishedGroundBelow(Vector3Int coordinates)
	{
		Vector3Int coords = coordinates - new Vector3Int(0, 0, 1);
		return _stackableBlockService.IsUnfinishedGroundBlockAt(coords);
	}
}
