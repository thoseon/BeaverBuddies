using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Navigation;

public class GateConflictDetector
{
	private readonly NodeIdService _nodeIdService;

	private readonly PreviewDistrictMap _previewDistrictMap;

	private readonly PreviewRoadNavMeshGraph _previewRoadNavMeshGraph;

	private readonly PreviewDistrictObstacleService _previewDistrictObstacleService;

	private readonly Queue<int> _nodesToVisit = new Queue<int>();

	private readonly HashSet<int> _visitedNodes = new HashSet<int>();

	private readonly List<int> _neighborNodes = new List<int>();

	private readonly HashSet<int> _ignorableNodes = new HashSet<int>();

	private readonly Dictionary<int, int> _nodeToDistrict = new Dictionary<int, int>();

	internal GateConflictDetector(NodeIdService nodeIdService, PreviewDistrictMap previewDistrictMap, PreviewRoadNavMeshGraph previewRoadNavMeshGraph, PreviewDistrictObstacleService previewDistrictObstacleService)
	{
		_nodeIdService = nodeIdService;
		_previewDistrictMap = previewDistrictMap;
		_previewRoadNavMeshGraph = previewRoadNavMeshGraph;
		_previewDistrictObstacleService = previewDistrictObstacleService;
	}

	public bool CanOpenGateWithoutConflict(Vector3Int from, Vector3Int to, Vector3Int center, Dictionary<Vector3Int, Vector3Int> openGateCrossings)
	{
		int num = _nodeIdService.GridToId(from);
		int num2 = _nodeIdService.GridToId(to);
		int item = _nodeIdService.GridToId(center);
		_ignorableNodes.Add(num);
		_ignorableNodes.Add(num2);
		_ignorableNodes.Add(item);
		int num3 = 0;
		foreach (int item2 in _previewDistrictMap.DistrictCenterNodeIds())
		{
			_nodeToDistrict.Add(item2, num3++);
		}
		int? num4 = FindDistrictId(num, openGateCrossings);
		int? num5 = FindDistrictId(num2, openGateCrossings);
		_ignorableNodes.Clear();
		_nodeToDistrict.Clear();
		if (!num4.HasValue || !num5.HasValue)
		{
			return true;
		}
		return num4.Value == num5.Value;
	}

	private int? FindDistrictId(int startNodeId, Dictionary<Vector3Int, Vector3Int> openGateCrossings)
	{
		_nodesToVisit.Clear();
		_visitedNodes.Clear();
		if (_previewDistrictObstacleService.IsSetObstacle(startNodeId))
		{
			return null;
		}
		_visitedNodes.Add(startNodeId);
		_nodesToVisit.Enqueue(startNodeId);
		while (_nodesToVisit.Count > 0)
		{
			int num = _nodesToVisit.Dequeue();
			if (_nodeToDistrict.TryGetValue(num, out var value))
			{
				return value;
			}
			VisitNode(num, openGateCrossings);
		}
		return null;
	}

	private void VisitNode(int nodeId, Dictionary<Vector3Int, Vector3Int> openGateCrossings)
	{
		foreach (NavMeshNode neighbor in _previewRoadNavMeshGraph.GetNeighbors(nodeId))
		{
			_neighborNodes.Add(neighbor.Id);
		}
		Vector3Int key = _nodeIdService.IdToGrid(nodeId);
		if (openGateCrossings.TryGetValue(key, out var value))
		{
			int item = _nodeIdService.GridToId(value);
			_neighborNodes.Add(item);
		}
		for (int i = 0; i < _neighborNodes.Count; i++)
		{
			int num = _neighborNodes[i];
			if (!_visitedNodes.Contains(num) && !_ignorableNodes.Contains(num) && !_previewDistrictObstacleService.IsSetObstacle(num))
			{
				_visitedNodes.Add(num);
				_nodesToVisit.Enqueue(num);
			}
		}
		_neighborNodes.Clear();
	}
}
