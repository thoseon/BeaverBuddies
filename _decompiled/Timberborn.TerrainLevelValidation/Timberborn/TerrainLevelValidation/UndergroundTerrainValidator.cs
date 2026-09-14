using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Localization;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.TerrainLevelValidation;

internal class UndergroundTerrainValidator : IBlockObjectValidator
{
	private static readonly string MissingGroundBelowLocKey = "Buildings.MissingGroundBelow";

	private readonly IBlockService _blockService;

	private readonly ILoc _loc;

	private readonly ITerrainService _terrainService;

	public UndergroundTerrainValidator(IBlockService blockService, ILoc loc, ITerrainService terrainService)
	{
		_blockService = blockService;
		_loc = loc;
		_terrainService = terrainService;
	}

	public bool IsValid(BlockObject blockObject, out string errorMessage)
	{
		foreach (Block allBlock in blockObject.PositionedBlocks.GetAllBlocks())
		{
			if (allBlock.MatterBelow == MatterBelow.Ground)
			{
				Vector3Int blockBelow = allBlock.Coordinates.Below();
				if (!_terrainService.Underground(blockBelow) && !_blockService.GetObjectsAt(blockBelow).FastAny((BlockObject foundObject) => IsUnfinishedGroundAtPosition(foundObject, blockBelow)))
				{
					errorMessage = _loc.T(MissingGroundBelowLocKey);
					return false;
				}
			}
		}
		errorMessage = null;
		return true;
	}

	private static bool IsUnfinishedGroundAtPosition(BlockObject blockObject, Vector3Int position)
	{
		if (blockObject.PositionedBlocks.TryGetBlock(position, out var result))
		{
			return result.Stackable == BlockStackable.UnfinishedGround;
		}
		return false;
	}
}
