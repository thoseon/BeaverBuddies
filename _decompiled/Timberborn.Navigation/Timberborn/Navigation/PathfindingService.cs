using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Navigation;

internal class PathfindingService
{
	private readonly TerrainFlowFieldCache _terrainFlowFieldCache;

	private readonly RoadFlowFieldCache _roadFlowFieldCache;

	private readonly TerrainAStarPathfinder _terrainAStarPathfinder;

	private readonly TerrainFlowFieldGenerator _terrainFlowFieldGenerator;

	private readonly RoadFlowFieldGenerator _roadFlowFieldGenerator;

	private readonly RoadAStarPathfinder _roadAStarPathfinder;

	private readonly NodeIdService _nodeIdService;

	private readonly TerrainNavMeshGraph _terrainNavMeshGraph;

	private readonly RoadNavMeshGraph _roadNavMeshGraph;

	private readonly InstantRoadNavMeshGraph _instantRoadNavMeshGraph;

	private readonly DistrictMap _districtMap;

	private readonly InstantDistrictMap _instantDistrictMap;

	private readonly FlowFieldPathFinder _flowFieldPathFinder;

	private readonly List<int> _destinationNodeIds = new List<int>();

	public PathfindingService(TerrainFlowFieldCache terrainFlowFieldCache, RoadFlowFieldCache roadFlowFieldCache, TerrainAStarPathfinder terrainAStarPathfinder, TerrainFlowFieldGenerator terrainFlowFieldGenerator, RoadFlowFieldGenerator roadFlowFieldGenerator, RoadAStarPathfinder roadAStarPathfinder, NodeIdService nodeIdService, TerrainNavMeshGraph terrainNavMeshGraph, RoadNavMeshGraph roadNavMeshGraph, InstantRoadNavMeshGraph instantRoadNavMeshGraph, DistrictMap districtMap, InstantDistrictMap instantDistrictMap, FlowFieldPathFinder flowFieldPathFinder)
	{
		_terrainFlowFieldCache = terrainFlowFieldCache;
		_roadFlowFieldCache = roadFlowFieldCache;
		_terrainAStarPathfinder = terrainAStarPathfinder;
		_terrainFlowFieldGenerator = terrainFlowFieldGenerator;
		_roadFlowFieldGenerator = roadFlowFieldGenerator;
		_roadAStarPathfinder = roadAStarPathfinder;
		_nodeIdService = nodeIdService;
		_terrainNavMeshGraph = terrainNavMeshGraph;
		_roadNavMeshGraph = roadNavMeshGraph;
		_instantRoadNavMeshGraph = instantRoadNavMeshGraph;
		_districtMap = districtMap;
		_instantDistrictMap = instantDistrictMap;
		_flowFieldPathFinder = flowFieldPathFinder;
	}

	public bool FindPathUncached(Vector3 start, Vector3 destination, out float distance, List<PathCorner> pathCorners = null)
	{
		if (FindRoadPathIfCachedAndFillIfNeeded(PathRequest.Create(start, destination), out distance, pathCorners))
		{
			return true;
		}
		if (FindRoadPathIfCachedAndFillIfNeeded(PathRequest.CreateReversed(destination, start), out distance, pathCorners))
		{
			return true;
		}
		if (FindTerrainPathIfCached(PathRequest.Create(start, destination), out distance, pathCorners))
		{
			return true;
		}
		if (FindTerrainPathIfCached(PathRequest.CreateReversed(destination, start), out distance, pathCorners))
		{
			return true;
		}
		if (FindRoadPathUncached(_roadNavMeshGraph, _roadFlowFieldCache.GetDefaultFlowField(), start, destination, out distance, pathCorners))
		{
			return true;
		}
		if (FindRoadSpillPathIfCached(PathRequest.Create(start, destination), out distance, pathCorners))
		{
			return true;
		}
		if (FindRoadSpillPathIfCached(PathRequest.CreateReversed(destination, start), out distance, pathCorners))
		{
			return true;
		}
		if (FindTerrainPathUncached(start, destination, out distance, pathCorners))
		{
			return true;
		}
		distance = 0f;
		pathCorners?.Clear();
		return false;
	}

	public bool FindPathUncached(Vector3 start, IReadOnlyList<Vector3> destinations, out float distance, List<PathCorner> pathCorners = null)
	{
		if (destinations.Count == 1)
		{
			return FindPathUncached(start, destinations[0], out distance, pathCorners);
		}
		if (FindRoadPathIfCached(start, destinations, out distance, pathCorners))
		{
			return true;
		}
		if (FindTerrainPathIfCached(start, destinations, out distance, pathCorners))
		{
			return true;
		}
		if (FindRoadSpillPathIfCached(start, destinations, out distance, pathCorners))
		{
			return true;
		}
		if (FindRoadOrTerrainPathUncached(start, destinations, out distance, pathCorners))
		{
			return true;
		}
		distance = 0f;
		pathCorners?.Clear();
		return false;
	}

	public bool FindRoadPathCached(Vector3 start, Vector3 destination, out float distance, List<PathCorner> pathCorners = null)
	{
		int num = _nodeIdService.WorldToId(start);
		AccessFlowField flowFieldAtNode = _roadFlowFieldCache.GetFlowFieldAtNode(num);
		AccessFlowField districtRoadFlowFieldByRoadNodeId = _districtMap.GetDistrictRoadFlowFieldByRoadNodeId(num);
		_roadFlowFieldGenerator.FillFlowField(_roadNavMeshGraph, flowFieldAtNode, districtRoadFlowFieldByRoadNodeId, num);
		return _flowFieldPathFinder.FindPathInFlowField(flowFieldAtNode, PathRequest.Create(start, destination), out distance, pathCorners);
	}

	public bool FindInstantRoadPath(Vector3 start, Vector3 destination, out float distance)
	{
		int num = _nodeIdService.WorldToId(start);
		AccessFlowField instantFlowField = _roadFlowFieldCache.GetInstantFlowField(num);
		AccessFlowField districtRoadFlowFieldByRoadNodeId = _instantDistrictMap.GetDistrictRoadFlowFieldByRoadNodeId(num);
		_roadFlowFieldGenerator.FillFlowField(_instantRoadNavMeshGraph, instantFlowField, districtRoadFlowFieldByRoadNodeId, num);
		return _flowFieldPathFinder.FindPathInFlowField(instantFlowField, PathRequest.Create(start, destination), out distance, null);
	}

	public bool FindTerrainPathCached(Vector3 start, Vector3 destination, float maxDistance, out float distance, List<PathCorner> pathCorners = null)
	{
		int num = _nodeIdService.WorldToId(start);
		AccessFlowField flowFieldAtNode = _terrainFlowFieldCache.GetFlowFieldAtNode(num);
		_terrainFlowFieldGenerator.FillFlowFieldUpToDistance(_terrainNavMeshGraph, flowFieldAtNode, maxDistance, num);
		return _flowFieldPathFinder.FindPathInFlowField(flowFieldAtNode, PathRequest.Create(start, destination), out distance, pathCorners);
	}

	public bool FindPathFromRoadToTerrainCached(Vector3 roadStart, Vector3 terrainDestination, out Vector3 endOfRoad, out float distanceFromClosestRoad, out float totalDistance)
	{
		int nodeId = _nodeIdService.WorldToId(roadStart);
		int nodeId2 = _nodeIdService.WorldToId(terrainDestination);
		RoadSpillFlowField districtRoadSpillFlowFieldByRoadNodeId = _districtMap.GetDistrictRoadSpillFlowFieldByRoadNodeId(nodeId);
		if (districtRoadSpillFlowFieldByRoadNodeId.HasNode(nodeId2))
		{
			int roadParentNodeId = districtRoadSpillFlowFieldByRoadNodeId.GetRoadParentNodeId(nodeId2);
			endOfRoad = _nodeIdService.IdToWorld(roadParentNodeId);
			if (FindRoadPathCached(roadStart, endOfRoad, out var distance))
			{
				distanceFromClosestRoad = districtRoadSpillFlowFieldByRoadNodeId.GetDistanceToRoad(nodeId2);
				totalDistance = distanceFromClosestRoad + distance;
				return true;
			}
		}
		endOfRoad = default(Vector3);
		distanceFromClosestRoad = 0f;
		totalDistance = 0f;
		return false;
	}

	public bool FindRoadSpillOrTerrainPath(Vector3 start, IReadOnlyList<Vector3> destinations, out float distance, List<PathCorner> pathCorners)
	{
		if (FindRoadSpillPathIfCached(start, destinations, out distance, pathCorners))
		{
			return true;
		}
		if (!FindTerrainPathIfCached(start, destinations, out distance, pathCorners))
		{
			_destinationNodeIds.Clear();
			for (int i = 0; i < destinations.Count; i++)
			{
				_destinationNodeIds.Add(_nodeIdService.WorldToId(destinations[i]));
			}
			return FindTerrainPathUncached(start, _destinationNodeIds, out distance, pathCorners);
		}
		distance = 0f;
		pathCorners?.Clear();
		return false;
	}

	private bool FindRoadPathIfCachedAndFillIfNeeded(PathRequest pathRequest, out float distance, List<PathCorner> pathCorners)
	{
		int num = _nodeIdService.WorldToId(pathRequest.Start);
		distance = 0f;
		if (_roadFlowFieldCache.TryGetFlowFieldAtNode(num, out var flowField) && (flowField.IsFilled || TryFillRoadFlowField(num, flowField)))
		{
			return _flowFieldPathFinder.FindPathInFlowField(flowField, pathRequest, out distance, pathCorners);
		}
		return false;
	}

	private bool TryFillRoadFlowField(int startNodeId, AccessFlowField flowField)
	{
		AccessFlowField districtRoadFlowFieldByRoadNodeId = _districtMap.GetDistrictRoadFlowFieldByRoadNodeId(startNodeId);
		if (districtRoadFlowFieldByRoadNodeId.IsFilled)
		{
			_roadFlowFieldGenerator.FillFlowField(_roadNavMeshGraph, flowField, districtRoadFlowFieldByRoadNodeId, startNodeId);
			return true;
		}
		return false;
	}

	private bool FindTerrainPathIfCached(PathRequest pathRequest, out float distance, List<PathCorner> pathCorners)
	{
		int nodeId = _nodeIdService.WorldToId(pathRequest.Start);
		distance = 0f;
		if (_terrainFlowFieldCache.TryGetFlowFieldAtNode(nodeId, out var flowField))
		{
			return _flowFieldPathFinder.FindPathInFlowField(flowField, pathRequest, out distance, pathCorners);
		}
		return false;
	}

	private bool FindRoadSpillPathIfCached(PathRequest pathRequest, out float distance, List<PathCorner> pathCorners = null)
	{
		int num = _nodeIdService.WorldToId(pathRequest.Start);
		int nodeId = _nodeIdService.WorldToId(pathRequest.Destination);
		RoadSpillFlowField districtRoadSpillFlowFieldByRoadNodeId = _districtMap.GetDistrictRoadSpillFlowFieldByRoadNodeId(num);
		if (districtRoadSpillFlowFieldByRoadNodeId != null && districtRoadSpillFlowFieldByRoadNodeId.HasNode(nodeId))
		{
			int roadParentNodeId = districtRoadSpillFlowFieldByRoadNodeId.GetRoadParentNodeId(nodeId);
			if (_roadFlowFieldCache.TryGetFlowFieldAtNode(num, out var flowField) && flowField.FoundPath(roadParentNodeId))
			{
				return _flowFieldPathFinder.FindPathInFlowField(flowField, districtRoadSpillFlowFieldByRoadNodeId, pathRequest, out distance, pathCorners);
			}
			PathFlowField defaultFlowField = _roadFlowFieldCache.GetDefaultFlowField();
			_roadAStarPathfinder.FillFlowFieldWithPath(_roadNavMeshGraph, defaultFlowField, num, roadParentNodeId);
			if (_flowFieldPathFinder.FindPathInFlowField(defaultFlowField, districtRoadSpillFlowFieldByRoadNodeId, pathRequest, out distance, pathCorners))
			{
				return true;
			}
		}
		distance = 0f;
		return false;
	}

	private bool FindRoadPathUncached(RoadNavMeshGraph roadNavMeshGraph, PathFlowField flowField, Vector3 start, Vector3 destination, out float distance, List<PathCorner> pathCorners = null)
	{
		int startNodeId = _nodeIdService.WorldToId(start);
		int destinationNodeId = _nodeIdService.WorldToId(destination);
		_roadAStarPathfinder.FillFlowFieldWithPath(roadNavMeshGraph, flowField, startNodeId, destinationNodeId);
		distance = 0f;
		return _flowFieldPathFinder.FindPathInFlowField(flowField, PathRequest.Create(start, destination), out distance, pathCorners);
	}

	private bool FindTerrainPathUncached(Vector3 start, Vector3 destination, out float distance, List<PathCorner> pathCorners = null)
	{
		int startNodeId = _nodeIdService.WorldToId(start);
		int destinationNodeId = _nodeIdService.WorldToId(destination);
		PathFlowField defaultFlowField = _terrainFlowFieldCache.GetDefaultFlowField();
		_terrainAStarPathfinder.FillFlowFieldWithPath(_terrainNavMeshGraph, defaultFlowField, startNodeId, destinationNodeId);
		distance = 0f;
		return _flowFieldPathFinder.FindPathInFlowField(defaultFlowField, PathRequest.Create(start, destination), out distance, pathCorners);
	}

	private bool FindRoadPathIfCached(Vector3 start, IReadOnlyList<Vector3> destinations, out float distance, List<PathCorner> pathCorners)
	{
		int nodeId = _nodeIdService.WorldToId(start);
		distance = 0f;
		if (_roadFlowFieldCache.TryGetFlowFieldAtNode(nodeId, out var flowField))
		{
			return _flowFieldPathFinder.FindPathInFlowField(start, destinations, flowField, out distance, pathCorners);
		}
		return false;
	}

	private bool FindTerrainPathIfCached(Vector3 start, IReadOnlyList<Vector3> destinations, out float distance, List<PathCorner> pathCorners)
	{
		int nodeId = _nodeIdService.WorldToId(start);
		distance = 0f;
		if (_terrainFlowFieldCache.TryGetFlowFieldAtNode(nodeId, out var flowField))
		{
			return _flowFieldPathFinder.FindPathInFlowField(start, destinations, flowField, out distance, pathCorners);
		}
		return false;
	}

	private bool FindRoadSpillPathIfCached(Vector3 start, IReadOnlyList<Vector3> destinations, out float distance, List<PathCorner> pathCorners = null)
	{
		int num = _nodeIdService.WorldToId(start);
		RoadSpillFlowField districtRoadSpillFlowFieldByRoadNodeId = _districtMap.GetDistrictRoadSpillFlowFieldByRoadNodeId(num);
		if (districtRoadSpillFlowFieldByRoadNodeId != null && districtRoadSpillFlowFieldByRoadNodeId.IsFilled)
		{
			Vector3? vector = null;
			float num2 = float.PositiveInfinity;
			for (int i = 0; i < destinations.Count; i++)
			{
				Vector3 vector2 = destinations[i];
				int nodeId = _nodeIdService.WorldToId(vector2);
				if (!districtRoadSpillFlowFieldByRoadNodeId.HasNode(nodeId))
				{
					continue;
				}
				float? num3 = null;
				if (_roadFlowFieldCache.TryGetFlowFieldAtNode(num, out var flowField) && _flowFieldPathFinder.FindPathInFlowField(flowField, districtRoadSpillFlowFieldByRoadNodeId, PathRequest.Create(start, vector2), out distance))
				{
					num3 = distance;
				}
				else
				{
					PathFlowField defaultFlowField = _roadFlowFieldCache.GetDefaultFlowField();
					int roadParentNodeId = districtRoadSpillFlowFieldByRoadNodeId.GetRoadParentNodeId(nodeId);
					_roadAStarPathfinder.FillFlowFieldWithPath(_roadNavMeshGraph, defaultFlowField, num, roadParentNodeId);
					if (_flowFieldPathFinder.FindPathInFlowField(defaultFlowField, districtRoadSpillFlowFieldByRoadNodeId, PathRequest.Create(start, vector2), out distance))
					{
						num3 = distance;
					}
				}
				if (num3 < num2)
				{
					vector = vector2;
					num2 = num3.Value;
				}
			}
			if (vector.HasValue)
			{
				return FindRoadSpillPathIfCached(PathRequest.Create(start, vector.Value), out distance, pathCorners);
			}
		}
		distance = 0f;
		return false;
	}

	private bool FindRoadOrTerrainPathUncached(Vector3 start, IReadOnlyList<Vector3> destinations, out float distance, List<PathCorner> pathCorners = null)
	{
		_destinationNodeIds.Clear();
		for (int i = 0; i < destinations.Count; i++)
		{
			_destinationNodeIds.Add(_nodeIdService.WorldToId(destinations[i]));
		}
		if (FindRoadPathUncached(start, _destinationNodeIds, out distance, pathCorners))
		{
			return true;
		}
		if (FindTerrainPathUncached(start, _destinationNodeIds, out distance, pathCorners))
		{
			return true;
		}
		distance = 0f;
		return false;
	}

	private bool FindRoadPathUncached(Vector3 start, IReadOnlyList<int> destinationNodeIds, out float distance, List<PathCorner> pathCorners = null)
	{
		int startNodeId = _nodeIdService.WorldToId(start);
		PathFlowField defaultFlowField = _roadFlowFieldCache.GetDefaultFlowField();
		if (_roadAStarPathfinder.FillFlowFieldWithPath(_roadNavMeshGraph, defaultFlowField, startNodeId, destinationNodeIds, out var destinationNodeId))
		{
			Vector3 destination = _nodeIdService.IdToWorld(destinationNodeId);
			return _flowFieldPathFinder.FindPathInFlowField(defaultFlowField, PathRequest.Create(start, destination), out distance, pathCorners);
		}
		distance = 0f;
		return false;
	}

	private bool FindTerrainPathUncached(Vector3 start, IReadOnlyList<int> destinationNodeIds, out float distance, List<PathCorner> pathCorners = null)
	{
		int startNodeId = _nodeIdService.WorldToId(start);
		PathFlowField defaultFlowField = _terrainFlowFieldCache.GetDefaultFlowField();
		if (_terrainAStarPathfinder.FillFlowFieldWithPath(_terrainNavMeshGraph, defaultFlowField, startNodeId, destinationNodeIds, out var destinationNodeId))
		{
			Vector3 destination = _nodeIdService.IdToWorld(destinationNodeId);
			return _flowFieldPathFinder.FindPathInFlowField(defaultFlowField, PathRequest.Create(start, destination), out distance, pathCorners);
		}
		distance = 0f;
		return false;
	}
}
