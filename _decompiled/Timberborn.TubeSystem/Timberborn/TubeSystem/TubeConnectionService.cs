using Timberborn.Coordinates;
using Timberborn.Navigation;
using Timberborn.PathSystem;
using UnityEngine;

namespace Timberborn.TubeSystem;

internal class TubeConnectionService
{
	private readonly INavMeshService _navMeshService;

	private readonly IPathService _pathService;

	public TubeConnectionService(INavMeshService navMeshService, IPathService pathService)
	{
		_navMeshService = navMeshService;
		_pathService = pathService;
	}

	public bool CanConnectInDirection(Vector3Int origin, Direction3D direction3D)
	{
		Vector3Int target = origin + direction3D.ToOffset();
		return CanConnect(origin, target);
	}

	private bool CanConnect(Vector3Int origin, Vector3Int target)
	{
		return IsPath(origin, target);
	}

	private bool IsPath(Vector3Int origin, Vector3Int target)
	{
		if (_navMeshService.AreConnectedRoadPreview(origin, target) && _pathService.IsPath(origin))
		{
			return _pathService.IsPath(target);
		}
		return false;
	}
}
