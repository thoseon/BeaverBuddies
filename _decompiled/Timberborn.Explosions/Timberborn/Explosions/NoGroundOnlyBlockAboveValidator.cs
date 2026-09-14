using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.Explosions;

internal class NoGroundOnlyBlockAboveValidator : IBlockObjectValidator
{
	private readonly IBlockService _blockService;

	public NoGroundOnlyBlockAboveValidator(IBlockService blockService)
	{
		_blockService = blockService;
	}

	public bool IsValid(BlockObject blockObject, out string errorMessage)
	{
		errorMessage = null;
		if ((bool)blockObject.GetComponent<Tunnel>())
		{
			Vector3Int coordinates = blockObject.Coordinates.Above();
			foreach (BlockObject item in _blockService.GetObjectsAt(coordinates))
			{
				if (item.GetComponent<IGroundMatterBelowInvalidator>() == null && item.PositionedBlocks.GetBlock(coordinates).MatterBelow == MatterBelow.Ground)
				{
					return false;
				}
			}
		}
		return true;
	}
}
