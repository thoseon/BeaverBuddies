using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.Navigation;

internal class DistrictConflictDetector
{
	private readonly struct AssignedNode
	{
		public int Id { get; }

		public int DistrictId { get; }

		public AssignedNode(int id, int districtId)
		{
			Id = id;
			DistrictId = districtId;
		}
	}

	private readonly Dictionary<int, int> _assignedNodes = new Dictionary<int, int>();

	private readonly Queue<AssignedNode> _enqueuedNodes = new Queue<AssignedNode>();

	public bool AreDistrictsInConflict(RoadNavMeshGraph roadNavMeshGraph, DistrictObstacleService districtObstacleService, IEnumerable<int> districtCenters)
	{
		_assignedNodes.Clear();
		_enqueuedNodes.Clear();
		int num = 0;
		foreach (int districtCenter in districtCenters)
		{
			EnqueueNode(districtObstacleService, districtCenter, num++);
		}
		while (!_enqueuedNodes.IsEmpty())
		{
			AssignedNode assignedNode = _enqueuedNodes.Dequeue();
			int id = assignedNode.Id;
			int districtId = assignedNode.DistrictId;
			ReadOnlyList<NavMeshNode> neighbors = roadNavMeshGraph.GetNeighbors(id);
			for (int i = 0; i < neighbors.Count; i++)
			{
				int id2 = neighbors[i].Id;
				if (_assignedNodes.TryGetValue(id2, out var value))
				{
					if (value != districtId)
					{
						return true;
					}
				}
				else
				{
					EnqueueNode(districtObstacleService, id2, districtId);
				}
			}
		}
		return false;
	}

	private void EnqueueNode(DistrictObstacleService districtObstacleService, int nodeId, int nodeDistrictId)
	{
		if (!districtObstacleService.IsSetObstacle(nodeId))
		{
			_assignedNodes[nodeId] = nodeDistrictId;
			_enqueuedNodes.Enqueue(new AssignedNode(nodeId, nodeDistrictId));
		}
	}
}
