using System.Collections.Generic;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.Navigation;

internal class FlowFieldPathFinder
{
	private readonly NodeIdService _nodeIdService;

	private readonly FlowFieldPathBuilder _flowFieldPathBuilder;

	private readonly FlowFieldPathTransformer _flowFieldPathTransformer;

	private readonly List<FlowFieldPathNode> _pathCache = new List<FlowFieldPathNode>();

	private readonly List<FlowFieldPathNode> _partialPathCache = new List<FlowFieldPathNode>();

	public FlowFieldPathFinder(NodeIdService nodeIdService, FlowFieldPathBuilder flowFieldPathBuilder, FlowFieldPathTransformer flowFieldPathTransformer)
	{
		_nodeIdService = nodeIdService;
		_flowFieldPathBuilder = flowFieldPathBuilder;
		_flowFieldPathTransformer = flowFieldPathTransformer;
	}

	public bool FindPathInFlowField(PathFlowField flowField, PathRequest pathRequest, out float distance, List<PathCorner> pathCorners)
	{
		int startNodeId = _nodeIdService.WorldToId(pathRequest.Start);
		int num = _nodeIdService.WorldToId(pathRequest.Destination);
		if (flowField.FoundPath(startNodeId, num))
		{
			if (pathCorners != null)
			{
				_flowFieldPathBuilder.BuildPath(flowField, pathRequest, _pathCache);
				TransformPathIntoPathCorners(_pathCache, pathCorners, pathRequest);
			}
			distance = flowField.GetDistance(num);
			return true;
		}
		pathCorners?.Clear();
		distance = 0f;
		return false;
	}

	public bool FindPathInFlowField(AccessFlowField flowField, PathRequest pathRequest, out float distance, List<PathCorner> pathCorners)
	{
		int num = _nodeIdService.WorldToId(pathRequest.Destination);
		if (flowField.FoundPath(num))
		{
			if (pathCorners != null)
			{
				_flowFieldPathBuilder.BuildPath(flowField, pathRequest, _pathCache);
				TransformPathIntoPathCorners(_pathCache, pathCorners, pathRequest);
			}
			distance = flowField.GetDistance(num);
			return true;
		}
		pathCorners?.Clear();
		distance = 0f;
		return false;
	}

	public bool FindPathInFlowField(AccessFlowField accessFlowField, RoadSpillFlowField roadSpillFlowField, PathRequest pathRequest, out float distance, List<PathCorner> pathCorners = null)
	{
		int nodeId = _nodeIdService.WorldToId(pathRequest.Destination);
		if (roadSpillFlowField.HasNode(nodeId))
		{
			int roadParentNodeId = roadSpillFlowField.GetRoadParentNodeId(nodeId);
			if (accessFlowField.FoundPath(roadParentNodeId))
			{
				if (pathCorners != null)
				{
					Vector3 vector = _nodeIdService.IdToWorld(roadParentNodeId);
					_flowFieldPathBuilder.BuildPath(accessFlowField, PathRequest.Create(pathRequest.Start, vector), _pathCache);
					_partialPathCache.Clear();
					_flowFieldPathBuilder.BuildPath(roadSpillFlowField, PathRequest.Create(vector, pathRequest.Destination), _partialPathCache);
					_pathCache.RemoveLast();
					_pathCache.AddRange(_partialPathCache);
					TransformPathIntoPathCorners(_pathCache, pathCorners, pathRequest);
				}
				distance = accessFlowField.GetDistance(roadParentNodeId) + roadSpillFlowField.GetDistanceToRoad(nodeId);
				return true;
			}
		}
		pathCorners?.Clear();
		distance = 0f;
		return false;
	}

	public bool FindPathInFlowField(PathFlowField pathFlowField, RoadSpillFlowField roadSpillFlowField, PathRequest pathRequest, out float distance, List<PathCorner> pathCorners = null)
	{
		int startNodeId = _nodeIdService.WorldToId(pathRequest.Start);
		int nodeId = _nodeIdService.WorldToId(pathRequest.Destination);
		if (roadSpillFlowField.HasNode(nodeId))
		{
			int roadParentNodeId = roadSpillFlowField.GetRoadParentNodeId(nodeId);
			if (pathFlowField.FoundPath(startNodeId, roadParentNodeId))
			{
				if (pathCorners != null)
				{
					Vector3 vector = _nodeIdService.IdToWorld(roadParentNodeId);
					_flowFieldPathBuilder.BuildPath(pathFlowField, PathRequest.Create(pathRequest.Start, vector), _pathCache);
					_partialPathCache.Clear();
					_flowFieldPathBuilder.BuildPath(roadSpillFlowField, PathRequest.Create(vector, pathRequest.Destination), _partialPathCache);
					_pathCache.RemoveLast();
					_pathCache.AddRange(_partialPathCache);
					TransformPathIntoPathCorners(_pathCache, pathCorners, pathRequest);
				}
				distance = pathFlowField.GetDistance(roadParentNodeId) + roadSpillFlowField.GetDistanceToRoad(nodeId);
				return true;
			}
		}
		pathCorners?.Clear();
		distance = 0f;
		return false;
	}

	public bool FindPathInFlowField(Vector3 start, IReadOnlyList<Vector3> destinations, AccessFlowField flowField, out float distance, List<PathCorner> pathCorners = null)
	{
		Vector3? vector = null;
		float num = float.PositiveInfinity;
		for (int i = 0; i < destinations.Count; i++)
		{
			Vector3 vector2 = destinations[i];
			int num2 = _nodeIdService.WorldToId(vector2);
			if (flowField.FoundPath(num2))
			{
				float distance2 = flowField.GetDistance(num2);
				if (distance2 < num)
				{
					vector = vector2;
					num = distance2;
				}
			}
		}
		if (vector.HasValue)
		{
			return FindPathInFlowField(flowField, PathRequest.Create(start, vector.Value), out distance, pathCorners);
		}
		distance = 0f;
		return false;
	}

	private void TransformPathIntoPathCorners(List<FlowFieldPathNode> flowFieldPath, List<PathCorner> pathCorners, PathRequest pathRequest)
	{
		if (pathRequest.Reversed)
		{
			_flowFieldPathTransformer.TransformReversedPath(flowFieldPath, pathCorners);
		}
		else
		{
			_flowFieldPathTransformer.TransformPath(flowFieldPath, pathCorners);
		}
	}
}
