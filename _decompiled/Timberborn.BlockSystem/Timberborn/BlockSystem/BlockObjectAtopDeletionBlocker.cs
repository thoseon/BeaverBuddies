using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.BlockSystem;

public class BlockObjectAtopDeletionBlocker : BaseComponent, IAwakableComponent, IBlockObjectDeletionBlocker
{
	private readonly IBlockService _blockService;

	private BlockObject _blockObject;

	public bool NoForcedDelete => false;

	public bool IsStackedDeletionBlocked => false;

	public bool IsDeletionBlocked => HasBlockingObjectAtop();

	public string ReasonLocKey => "DeletionBlocker.ObjectAtop";

	public BlockObjectAtopDeletionBlocker(IBlockService blockService)
	{
		_blockService = blockService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	private bool HasBlockingObjectAtop()
	{
		foreach (Block occupiedStackableBlock in _blockObject.PositionedBlocks.GetOccupiedStackableBlocks())
		{
			Vector3Int coordinates = occupiedStackableBlock.Coordinates.Above();
			foreach (BlockObject item in _blockService.GetStackedObjectsWithUndergroundAt(coordinates))
			{
				if (item != _blockObject && !item.Overridable)
				{
					return true;
				}
			}
		}
		return false;
	}
}
