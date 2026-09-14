using System;
using Timberborn.SingletonSystem;

namespace Timberborn.Navigation;

internal class DistrictObstacleService : ILoadableSingleton
{
	private readonly NodeIdService _nodeIdService;

	private bool[] _obstacles;

	public DistrictObstacleService(NodeIdService nodeIdService)
	{
		_nodeIdService = nodeIdService;
	}

	public void Load()
	{
		_obstacles = new bool[_nodeIdService.NumberOfNodes];
	}

	public void SetObstacle(int nodeId)
	{
		if (IsSetObstacle(nodeId))
		{
			throw new InvalidOperationException($"Can't set obstacle at {nodeId}");
		}
		_obstacles[nodeId] = true;
	}

	public void UnsetObstacle(int nodeId)
	{
		if (!IsSetObstacle(nodeId))
		{
			throw new InvalidOperationException($"Can't unset obstacle at {nodeId}");
		}
		_obstacles[nodeId] = false;
	}

	public bool IsSetObstacle(int nodeId)
	{
		return _obstacles[nodeId];
	}
}
