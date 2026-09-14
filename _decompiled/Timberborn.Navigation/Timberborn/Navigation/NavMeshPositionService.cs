using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.Navigation;

internal class NavMeshPositionService
{
	private readonly INavMeshService _navMeshService;

	private readonly TerrainNavMeshGraph _terrainNavMeshGraph;

	private readonly NodeIdService _nodeIdService;

	private readonly Queue<Vector3Int> _nodeQueue = new Queue<Vector3Int>();

	private readonly HashSet<Vector3Int> _visitedNodes = new HashSet<Vector3Int>();

	public NavMeshPositionService(INavMeshService navMeshService, TerrainNavMeshGraph terrainNavMeshGraph, NodeIdService nodeIdService)
	{
		_navMeshService = navMeshService;
		_terrainNavMeshGraph = terrainNavMeshGraph;
		_nodeIdService = nodeIdService;
	}

	public Vector3? ClosestPositionOnNavMesh(Vector3 originWorld, float maxDistance)
	{
		Vector3Int vector3Int = NavigationCoordinateSystem.WorldToGridInt(originWorld);
		_nodeQueue.Clear();
		_visitedNodes.Clear();
		EnqueueNode(vector3Int);
		while (!_nodeQueue.IsEmpty())
		{
			Vector3Int vector3Int2 = _nodeQueue.Dequeue();
			int nodeId = _nodeIdService.GridToId(vector3Int2);
			if (_navMeshService.IsOnNavMesh(vector3Int2) && _terrainNavMeshGraph.IsConnectedToDefaultGroup(nodeId))
			{
				return NavigationCoordinateSystem.GridToWorld(vector3Int2);
			}
			EnqueueNeighbors(vector3Int, vector3Int2, maxDistance);
		}
		return null;
	}

	private void EnqueueNeighbors(Vector3Int origin, Vector3Int navMeshCoords, float maxDistance)
	{
		Vector3Int[] neighbors6Vector3Int = Deltas.Neighbors6Vector3Int;
		foreach (Vector3Int vector3Int in neighbors6Vector3Int)
		{
			Vector3Int vector3Int2 = navMeshCoords + vector3Int;
			if (Vector3Int.Distance(origin, vector3Int2) <= maxDistance)
			{
				EnqueueNode(vector3Int2);
			}
		}
	}

	private void EnqueueNode(Vector3Int nodeCoords)
	{
		if (!_visitedNodes.Contains(nodeCoords))
		{
			_nodeQueue.Enqueue(nodeCoords);
			_visitedNodes.Add(nodeCoords);
		}
	}
}
