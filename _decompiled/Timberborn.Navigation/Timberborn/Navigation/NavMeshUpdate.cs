using System.Collections.Generic;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.Navigation;

public readonly struct NavMeshUpdate
{
	internal class Builder
	{
		private readonly NodeIdService _nodeIdService;

		private readonly HashSet<int> _uniqueTerrainNodeIds = new HashSet<int>();

		private readonly List<int> _terrainNodeIds = new List<int>();

		private readonly List<Vector3Int> _terrainCoordinates = new List<Vector3Int>();

		private readonly HashSet<int> _uniqueRoadNodeIds = new HashSet<int>();

		private readonly List<int> _roadNodeIds = new List<int>();

		private BoundingBox.Builder _boundingBoxBuilder;

		public bool IsEmpty
		{
			get
			{
				if (_uniqueTerrainNodeIds.IsEmpty())
				{
					return _uniqueRoadNodeIds.IsEmpty();
				}
				return false;
			}
		}

		public Builder(NodeIdService nodeIdService)
		{
			_nodeIdService = nodeIdService;
		}

		public void Reset()
		{
			_boundingBoxBuilder = default(BoundingBox.Builder);
			_uniqueTerrainNodeIds.Clear();
			_terrainNodeIds.Clear();
			_terrainCoordinates.Clear();
			_uniqueRoadNodeIds.Clear();
			_roadNodeIds.Clear();
		}

		public void AddTerrainNode(int nodeId)
		{
			if (_uniqueTerrainNodeIds.Add(nodeId))
			{
				_terrainNodeIds.Add(nodeId);
				Vector3Int vector3Int = _nodeIdService.IdToGrid(nodeId);
				_terrainCoordinates.Add(vector3Int);
				_boundingBoxBuilder.Expand(vector3Int);
			}
		}

		public void AddRoadNode(int nodeId)
		{
			if (_uniqueRoadNodeIds.Add(nodeId))
			{
				_roadNodeIds.Add(nodeId);
				Vector3Int point = _nodeIdService.IdToGrid(nodeId);
				_boundingBoxBuilder.Expand(point);
			}
		}

		public NavMeshUpdate Build()
		{
			return new NavMeshUpdate(_boundingBoxBuilder.Build(), _terrainCoordinates.AsReadOnlyList(), _terrainNodeIds.AsReadOnlyList(), _roadNodeIds.AsReadOnlyList());
		}
	}

	public BoundingBox Bounds { get; }

	public ReadOnlyList<Vector3Int> TerrainCoordinates { get; }

	internal ReadOnlyList<int> TerrainNodeIds { get; }

	internal ReadOnlyList<int> RoadNodeIds { get; }

	public bool UpdatedRoads => !RoadNodeIds.IsEmpty();

	private NavMeshUpdate(BoundingBox bounds, ReadOnlyList<Vector3Int> terrainCoordinates, ReadOnlyList<int> terrainNodeIds, ReadOnlyList<int> roadNodeIds)
	{
		Bounds = bounds;
		TerrainCoordinates = terrainCoordinates;
		TerrainNodeIds = terrainNodeIds;
		RoadNodeIds = roadNodeIds;
	}
}
