using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Navigation;

internal class FlowFieldPathBuilder
{
	private readonly TerrainNavMeshGraph _terrainNavMeshGraph;

	private readonly NodeIdService _nodeIdService;

	public FlowFieldPathBuilder(TerrainNavMeshGraph terrainNavMeshGraph, NodeIdService nodeIdService)
	{
		_terrainNavMeshGraph = terrainNavMeshGraph;
		_nodeIdService = nodeIdService;
	}

	public void BuildPath(IFlowField flowField, PathRequest pathRequest, List<FlowFieldPathNode> flowFieldPath)
	{
		flowFieldPath.Clear();
		if (pathRequest.Destination == pathRequest.Start)
		{
			flowFieldPath.Add(new FlowFieldPathNode(pathRequest.Destination, 0f, 0f, 0));
		}
		else
		{
			BuildPathInternal(flowField, pathRequest, flowFieldPath);
		}
	}

	private void BuildPathInternal(IFlowField flowField, PathRequest pathRequest, List<FlowFieldPathNode> flowFieldPath)
	{
		flowFieldPath.Add(new FlowFieldPathNode(pathRequest.Destination, 0f, 0f, 0));
		int num = _nodeIdService.WorldToId(pathRequest.Start);
		int num2 = _nodeIdService.WorldToId(pathRequest.Destination);
		int previousNodeId = num2;
		if (num != num2)
		{
			int num3 = flowField.GetParentId(num2);
			while (num3 != num)
			{
				AddEdgeNode(num3, previousNodeId, flowFieldPath);
				int parentId = flowField.GetParentId(num3);
				if (parentId == num3)
				{
					throw new InvalidOperationException($"Infinite loop at {num3} {_nodeIdService.IdToGrid(num3)}," + $" start: {num} {_nodeIdService.IdToGrid(num)}," + $" destination: {num2} {_nodeIdService.IdToGrid(num2)}");
				}
				previousNodeId = num3;
				num3 = parentId;
			}
		}
		AppendStartPoint(pathRequest.Start, num, previousNodeId, flowFieldPath);
		flowFieldPath.Reverse();
	}

	private void AddEdgeNode(int nodeId, int previousNodeId, List<FlowFieldPathNode> flowFieldPath)
	{
		float connectionCost = _terrainNavMeshGraph.GetConnectionCost(nodeId, previousNodeId);
		float distanceToNext = _nodeIdService.Distance(nodeId, previousNodeId);
		int groupId = _terrainNavMeshGraph.GetGroupId(nodeId, previousNodeId);
		flowFieldPath.Add(new FlowFieldPathNode(_nodeIdService.IdToWorld(nodeId), connectionCost, distanceToNext, groupId));
	}

	private void AppendStartPoint(Vector3 start, int startNodeId, int previousNodeId, List<FlowFieldPathNode> flowFieldPath)
	{
		FlowFieldPathNode flowFieldPathNode = flowFieldPath[flowFieldPath.Count - 1];
		float num = Vector3.Distance(start, flowFieldPathNode.Position);
		bool num2 = flowFieldPath.Count > 1;
		float cost = (num2 ? _terrainNavMeshGraph.GetConnectionCost(startNodeId, previousNodeId) : Math.Min(1f, 1f / num));
		int groupId = (num2 ? _terrainNavMeshGraph.GetGroupId(startNodeId, previousNodeId) : flowFieldPathNode.GroupId);
		flowFieldPath.Add(new FlowFieldPathNode(start, cost, num, groupId));
	}
}
