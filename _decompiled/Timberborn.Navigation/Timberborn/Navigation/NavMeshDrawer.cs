using System.Collections.Generic;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.Navigation;

internal class NavMeshDrawer : INavMeshDrawer, IPrioritizedSingletonNavMeshListener
{
	private readonly NodeIdService _nodeIdService;

	private readonly TerrainNavMeshGraph _terrainNavMeshGraph;

	private readonly RestrictedNodeMap _restrictedNodeMap;

	private readonly HashSet<int> _nodesWithNeighbors = new HashSet<int>();

	public NavMeshDrawer(NodeIdService nodeIdService, TerrainNavMeshGraph terrainNavMeshGraph, RestrictedNodeMap restrictedNodeMap)
	{
		_nodeIdService = nodeIdService;
		_terrainNavMeshGraph = terrainNavMeshGraph;
		_restrictedNodeMap = restrictedNodeMap;
	}

	public void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		UpdateNodes(navMeshUpdate.TerrainNodeIds);
	}

	public void DrawForOneFrameAroundCoordinates(Vector3Int coordinates)
	{
		Vector3 center = NavigationCoordinateSystem.GridToWorld(coordinates);
		foreach (int nodesWithNeighbor in _nodesWithNeighbors)
		{
			DrawNodeAndItsEdges(center, nodesWithNeighbor);
		}
	}

	private void UpdateNodes(ReadOnlyList<int> nodeIds)
	{
		for (int i = 0; i < nodeIds.Count; i++)
		{
			UpdateNode(nodeIds[i]);
		}
	}

	private void UpdateNode(int nodeId)
	{
		if (_terrainNavMeshGraph.GetNeighbors(nodeId).IsEmpty())
		{
			_nodesWithNeighbors.Remove(nodeId);
		}
		else
		{
			_nodesWithNeighbors.Add(nodeId);
		}
	}

	private void DrawNodeAndItsEdges(Vector3 center, int nodeId)
	{
		Vector3 vector = _nodeIdService.IdToWorld(nodeId);
		if (Vector3.Distance(center, vector) < 30f)
		{
			Color color = (_restrictedNodeMap.IsNodeRestricted(nodeId) ? Color.yellow : Color.cyan);
			DrawNode(vector, color);
			DrawEdges(nodeId, vector);
		}
	}

	private static void DrawNode(Vector3 nodePosition, Color color)
	{
		Debug.DrawRay(nodePosition, Vector3.up / 3f, color);
	}

	private void DrawEdges(int nodeId, Vector3 nodePosition)
	{
		foreach (NavMeshNode neighbor in _terrainNavMeshGraph.GetNeighbors(nodeId))
		{
			Vector3 end = _nodeIdService.IdToWorld(neighbor.Id);
			Debug.DrawLine(nodePosition, end, Color.red);
		}
	}
}
