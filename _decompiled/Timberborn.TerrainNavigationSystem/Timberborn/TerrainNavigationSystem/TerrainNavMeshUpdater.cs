using System;
using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.Navigation;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.TerrainNavigationSystem;

public class TerrainNavMeshUpdater : ILoadableSingleton
{
	private readonly ITerrainService _terrainService;

	private readonly INavMeshService _navMeshService;

	private readonly NavMeshGroupService _navMeshGroupService;

	private readonly List<NavMeshEdge> _validNeighbourEdgeCache = new List<NavMeshEdge>();

	public TerrainNavMeshUpdater(ITerrainService terrainService, INavMeshService navMeshService, NavMeshGroupService navMeshGroupService)
	{
		_terrainService = terrainService;
		_navMeshService = navMeshService;
		_navMeshGroupService = navMeshGroupService;
	}

	public void Load()
	{
		AddTerrainToNavMesh();
		_terrainService.PreTerrainHeightChanged += OnPreTerrainHeightChanged;
		_terrainService.TerrainHeightChanged += OnTerrainHeightChanged;
	}

	private void OnPreTerrainHeightChanged(object sender, TerrainHeightChangeEventArgs terrainHeightChangeEventArgs)
	{
		TerrainHeightChange change = terrainHeightChangeEventArgs.Change;
		for (int i = change.From; i <= change.To + 1; i++)
		{
			Vector3Int vector3Int = change.Coordinates.ToVector3Int(i);
			Vector3Int[] neighbors8AndSelfVector3Int = Deltas.Neighbors8AndSelfVector3Int;
			foreach (Vector3Int vector3Int2 in neighbors8AndSelfVector3Int)
			{
				Vector3Int coordinates = vector3Int + vector3Int2;
				if (TileIsWalkable(coordinates))
				{
					RemoveEdgesToNeighbors(coordinates);
				}
			}
			UpdateBlockingEdges(vector3Int, block: false, i < change.To + 1);
		}
	}

	private void OnTerrainHeightChanged(object sender, TerrainHeightChangeEventArgs terrainHeightChangeEventArgs)
	{
		TerrainHeightChange change = terrainHeightChangeEventArgs.Change;
		for (int i = change.From; i <= change.To + 1; i++)
		{
			Vector3Int vector3Int = change.Coordinates.ToVector3Int(i);
			Vector3Int[] neighbors8AndSelfVector3Int = Deltas.Neighbors8AndSelfVector3Int;
			foreach (Vector3Int vector3Int2 in neighbors8AndSelfVector3Int)
			{
				Vector3Int coordinates = vector3Int + vector3Int2;
				if (TileIsWalkable(coordinates))
				{
					AddEdgesToNeighbors(coordinates);
				}
			}
			UpdateBlockingEdges(vector3Int, block: true, i < change.To + 1);
		}
	}

	private void AddTerrainToNavMesh()
	{
		Vector3Int size = _terrainService.Size;
		for (int i = 0; i < size.x; i++)
		{
			for (int j = 0; j < size.y; j++)
			{
				for (int k = 0; k < size.z; k++)
				{
					Vector3Int vector3Int = new Vector3Int(i, j, k);
					if (_terrainService.OnGround(vector3Int))
					{
						AddEdgesToNeighbors(vector3Int);
					}
					else if (_terrainService.Underground(vector3Int))
					{
						UpdateBlockingEdges(vector3Int, block: true);
					}
				}
			}
		}
	}

	private void AddEdgesToNeighbors(Vector3Int coordinates)
	{
		GetValidEdgesToNeighbors(coordinates);
		foreach (NavMeshEdge item in _validNeighbourEdgeCache)
		{
			_navMeshService.AddEdge(item);
		}
		_validNeighbourEdgeCache.Clear();
	}

	private void RemoveEdgesToNeighbors(Vector3Int coordinates)
	{
		GetValidEdgesToNeighbors(coordinates);
		foreach (NavMeshEdge item in _validNeighbourEdgeCache)
		{
			_navMeshService.RemoveEdge(item);
		}
		_validNeighbourEdgeCache.Clear();
	}

	private bool TileIsWalkable(Vector3Int coordinates)
	{
		return _terrainService.OnGround(coordinates);
	}

	private void GetValidEdgesToNeighbors(Vector3Int coordinates)
	{
		GetEdgesToNeighbors(coordinates, Deltas.Neighbors4Vector3Int, TilesAreOrthogonallyConnected);
		GetEdgesToNeighbors(coordinates, Deltas.Corners4Vector3Int, TilesAreDiagonallyConnected);
	}

	private void GetEdgesToNeighbors(Vector3Int coordinates, Vector3Int[] neighborDeltas, Func<Vector3Int, Vector3Int, bool> tilesAreConnected)
	{
		foreach (Vector3Int vector3Int in neighborDeltas)
		{
			Vector3Int vector3Int2 = coordinates + vector3Int;
			if (tilesAreConnected(coordinates, vector3Int2))
			{
				NavMeshEdge item = NavMeshEdge.CreateDefault(coordinates, vector3Int2);
				_validNeighbourEdgeCache.Add(item);
			}
		}
	}

	private bool TilesAreOrthogonallyConnected(Vector3Int coordinates, Vector3Int neighborCoordinates)
	{
		return HeightAtCoordinatesIsLessThanOrEqualTo(neighborCoordinates, coordinates.z);
	}

	private bool HeightAtCoordinatesIsLessThanOrEqualTo(Vector3Int coordinates, int height)
	{
		if (_terrainService.TryGetRelativeHeight(coordinates, out var relativeHeight))
		{
			return coordinates.z + relativeHeight <= height;
		}
		return false;
	}

	private bool TilesAreDiagonallyConnected(Vector3Int coordinates, Vector3Int neighborCoordinates)
	{
		Vector3Int vector3Int = neighborCoordinates - coordinates;
		Vector3Int vector3Int2 = new Vector3Int(vector3Int.x, 0, 0);
		Vector3Int vector3Int3 = new Vector3Int(0, vector3Int.y, 0);
		if (_terrainService.OnGround(neighborCoordinates) && _terrainService.OnGround(coordinates + vector3Int2))
		{
			return _terrainService.OnGround(coordinates + vector3Int3);
		}
		return false;
	}

	private void UpdateBlockingEdges(Vector3Int coordinates, bool block, bool updateNeighbors)
	{
		UpdateBlockingEdges(coordinates, block);
		if (updateNeighbors)
		{
			Vector3Int[] neighbors4Vector3Int = Deltas.Neighbors4Vector3Int;
			foreach (Vector3Int vector3Int in neighbors4Vector3Int)
			{
				UpdateBlockingEdges(coordinates + vector3Int, block);
			}
		}
	}

	private void UpdateBlockingEdges(Vector3Int coordinates, bool block)
	{
		if (!_terrainService.Underground(coordinates))
		{
			return;
		}
		bool flag = _terrainService.Underground(coordinates.Below());
		Vector3Int[] neighbors4Vector3Int = Deltas.Neighbors4Vector3Int;
		foreach (Vector3Int vector3Int in neighbors4Vector3Int)
		{
			Vector3Int vector3Int2 = coordinates + vector3Int;
			if (!flag || !_terrainService.Underground(vector3Int2))
			{
				UpdateBlockingEdge(coordinates, vector3Int2, block);
			}
		}
	}

	private void UpdateBlockingEdge(Vector3Int start, Vector3Int end, bool block)
	{
		foreach (int allGroupId in _navMeshGroupService.GetAllGroupIds())
		{
			NavMeshEdge navMeshEdge = NavMeshEdge.CreateBlocking(start, end, allGroupId);
			if (block)
			{
				_navMeshService.BlockEdge(navMeshEdge);
			}
			else
			{
				_navMeshService.UnblockEdge(navMeshEdge);
			}
		}
	}
}
