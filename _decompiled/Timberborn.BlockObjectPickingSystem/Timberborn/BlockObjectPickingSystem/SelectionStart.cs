using UnityEngine;

namespace Timberborn.BlockObjectPickingSystem;

public readonly struct SelectionStart
{
	private readonly BlockObjectHit? _originalBlockObjectHit;

	public Vector3Int Coordinates { get; }

	public float HitLevel { get; }

	public int VerticalOffset { get; }

	private SelectionStart(Vector3Int coordinates, float hitLevel, BlockObjectHit? originalBlockObjectHit, int verticalOffset)
	{
		Coordinates = coordinates;
		HitLevel = hitLevel;
		_originalBlockObjectHit = originalBlockObjectHit;
		VerticalOffset = verticalOffset;
	}

	public SelectionStart(Vector3Int coordinates)
		: this(coordinates, coordinates.z, null, 0)
	{
	}

	public SelectionStart(BlockObjectHit blockObjectHit)
		: this(blockObjectHit.HitProjectedOnGround, blockObjectHit.HitPoint.y, blockObjectHit, 0)
	{
		if (blockObjectHit.BlockObject.PositionedBlocks.GetBlock(Coordinates).Underground)
		{
			VerticalOffset = -1;
		}
	}

	public BlockObjectHit? GetBlockObjectHit()
	{
		if (_originalBlockObjectHit.HasValue && (bool)_originalBlockObjectHit.Value.BlockObject && _originalBlockObjectHit.Value.BlockObject.PositionedBlocks.HasBlockAt(Coordinates))
		{
			return _originalBlockObjectHit;
		}
		return null;
	}
}
