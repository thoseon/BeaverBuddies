using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.BlockingSystem;

public class BlockObjectBelowBlocker : BaseComponent, IAwakableComponent
{
	private readonly IBlockService _blockService;

	private BlockObject _blockObject;

	private readonly List<BlockableObject> _blockableObjectsBelow = new List<BlockableObject>();

	private bool _isBlocked;

	public BlockObjectBelowBlocker(IBlockService blockService)
	{
		_blockService = blockService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public void Block()
	{
		Asserts.IsFalse(this, _isBlocked, "_isBlocked");
		FillBlockableObjectsBelow();
		foreach (BlockableObject item in _blockableObjectsBelow)
		{
			item.Block(this);
		}
		_isBlocked = true;
	}

	public void Unblock()
	{
		Asserts.IsTrue(this, _isBlocked, "_isBlocked");
		foreach (BlockableObject item in _blockableObjectsBelow)
		{
			item.Unblock(this);
		}
		_blockableObjectsBelow.Clear();
		_isBlocked = false;
	}

	private void FillBlockableObjectsBelow()
	{
		foreach (Vector3Int foundationCoordinate in _blockObject.PositionedBlocks.GetFoundationCoordinates())
		{
			foreach (BlockableObject item in _blockService.GetObjectsWithComponentAt<BlockableObject>(foundationCoordinate.Below()))
			{
				_blockableObjectsBelow.Add(item);
			}
		}
	}
}
