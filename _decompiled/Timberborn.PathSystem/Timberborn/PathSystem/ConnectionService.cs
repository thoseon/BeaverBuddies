using System;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.PathSystem;

internal class ConnectionService : IConnectionService
{
	private readonly IBlockService _blockService;

	private readonly PreviewBlockService _previewBlockService;

	private readonly INavMeshService _navMeshService;

	private readonly IPathService _pathService;

	public ConnectionService(IBlockService blockService, PreviewBlockService previewBlockService, INavMeshService navMeshService, IPathService pathService)
	{
		_blockService = blockService;
		_previewBlockService = previewBlockService;
		_navMeshService = navMeshService;
		_pathService = pathService;
	}

	public bool CanConnectInDirection(Vector3Int origin, Direction2D direction2D)
	{
		Vector3Int target = origin + direction2D.ToOffset();
		return CanConnect(origin, target);
	}

	private bool CanConnect(Vector3Int origin, Vector3Int target)
	{
		if (!IsPath(origin, target) && !IsEntrance(origin, target))
		{
			return IsEnforced(origin, target);
		}
		return true;
	}

	private bool IsPath(Vector3Int origin, Vector3Int target)
	{
		if (_navMeshService.AreConnectedRoadPreview(origin, target) && _pathService.IsPath(origin))
		{
			return _pathService.IsPath(target);
		}
		return false;
	}

	private bool IsEntrance(Vector3Int origin, Vector3Int target)
	{
		if (!IsEntranceInDirectionAt(origin, target))
		{
			return IsEntranceInDirectionAt(target, origin);
		}
		return true;
	}

	private bool IsEntranceInDirectionAt(Vector3Int entranceCoordinates, Vector3Int doorstepCoordinates)
	{
		if (_navMeshService.AreConnectedPreview(entranceCoordinates, doorstepCoordinates) && _pathService.IsPath(entranceCoordinates))
		{
			Direction2D direction2D = ToEntranceDirection(entranceCoordinates - doorstepCoordinates);
			return ((_blockService.GetEntrancesAt(entranceCoordinates) | _previewBlockService.GetEntrancesAt(entranceCoordinates)) & direction2D.ToDirections()) != 0;
		}
		return false;
	}

	private static Direction2D ToEntranceDirection(Vector3Int direction)
	{
		if (direction == Direction2D.Down.ToOffset())
		{
			return Direction2D.Down;
		}
		if (direction == Direction2D.Left.ToOffset())
		{
			return Direction2D.Left;
		}
		if (direction == Direction2D.Right.ToOffset())
		{
			return Direction2D.Right;
		}
		if (direction == Direction2D.Up.ToOffset())
		{
			return Direction2D.Up;
		}
		throw new ArgumentOutOfRangeException("direction", direction, null);
	}

	private bool IsEnforced(Vector3Int origin, Vector3Int target)
	{
		IPathConnectionEnforcer obj = _blockService.GetFirstObjectWithComponentAt<IPathConnectionEnforcer>(origin) ?? _previewBlockService.GetFirstObjectWithComponentAt<IPathConnectionEnforcer>(origin);
		IPathConnectionEnforcer pathConnectionEnforcer = _blockService.GetFirstObjectWithComponentAt<IPathConnectionEnforcer>(target) ?? _previewBlockService.GetFirstObjectWithComponentAt<IPathConnectionEnforcer>(target);
		return obj?.CanConnectPath(origin, target) ?? pathConnectionEnforcer?.CanConnectPath(origin, target) ?? false;
	}
}
