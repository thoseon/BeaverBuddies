using System;
using System.Collections.Frozen;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using UnityEngine;

namespace Timberborn.BlockObjectAccesses;

internal class BlockObjectAccesses : BaseComponent, IAwakableComponent
{
	private BlockObject _blockObject;

	private BlockObjectAccessesSpec _blockObjectAccessesSpec;

	private Lazy<FrozenSet<Vector3Int>> _positionedBlockingCoordinates;

	private Lazy<FrozenSet<Vector3Int>> _positionedAllowedCoordinates;

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_blockObjectAccessesSpec = GetComponent<BlockObjectAccessesSpec>();
		_positionedBlockingCoordinates = new Lazy<FrozenSet<Vector3Int>>(PositionBlockingCoordinates);
		_positionedAllowedCoordinates = new Lazy<FrozenSet<Vector3Int>>(PositionAllowedCoordinates);
	}

	public bool IsBlocked(Vector3Int position)
	{
		return _positionedBlockingCoordinates.Value.Contains(position);
	}

	public bool IsAllowed(Vector3Int position)
	{
		return _positionedAllowedCoordinates.Value.Contains(position);
	}

	private FrozenSet<Vector3Int> PositionBlockingCoordinates()
	{
		return _blockObjectAccessesSpec.BlockingCoordinates.Select((Vector3Int coordinate) => _blockObject.TransformCoordinates(coordinate)).ToFrozenSet();
	}

	private FrozenSet<Vector3Int> PositionAllowedCoordinates()
	{
		return _blockObjectAccessesSpec.AllowedCoordinates.Select((Vector3Int coordinate) => _blockObject.TransformCoordinates(coordinate)).ToFrozenSet();
	}
}
