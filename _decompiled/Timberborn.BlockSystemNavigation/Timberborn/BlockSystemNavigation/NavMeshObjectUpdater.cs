using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.BlockSystemNavigation;

internal class NavMeshObjectUpdater
{
	private readonly NavMeshGroupService _navMeshGroupService;

	private readonly HashSet<NavMeshEdge> _addedEdges = new HashSet<NavMeshEdge>();

	private readonly HashSet<Vector3Int> _bottomOccupiedCoordinates = new HashSet<Vector3Int>();

	private readonly HashSet<Vector3Int> _topOccupiedCoordinates = new HashSet<Vector3Int>();

	private readonly HashSet<Vector3Int> _coordinatesBelowEntranceLevel = new HashSet<Vector3Int>();

	private readonly HashSet<NavMeshEdge> _blockedEdges = new HashSet<NavMeshEdge>();

	private readonly HashSet<NavMeshEdge> _unblockedEdges = new HashSet<NavMeshEdge>();

	public NavMeshObjectUpdater(NavMeshGroupService navMeshGroupService)
	{
		_navMeshGroupService = navMeshGroupService;
	}

	public void Update(BlockObject blockObject, NavMeshObject navMeshObject, BlockObjectNavMeshSettingsSpec blockObjectNavMeshSettingsSpec)
	{
		navMeshObject.Reset();
		if (blockObjectNavMeshSettingsSpec != null)
		{
			AddManuallySetEdges(blockObject, blockObjectNavMeshSettingsSpec);
			AddEdgesAddedByStackable(blockObject, blockObjectNavMeshSettingsSpec);
			foreach (NavMeshEdge addedEdge in _addedEdges)
			{
				navMeshObject.AddEdge(addedEdge);
			}
		}
		foreach (Block occupiedBlock in blockObject.PositionedBlocks.GetOccupiedBlocks())
		{
			Vector3Int coordinates = occupiedBlock.Coordinates;
			if (occupiedBlock.Occupation.Intersects(BlockOccupations.Bottom))
			{
				navMeshObject.AddRestrictedCoordinates(coordinates);
				_bottomOccupiedCoordinates.Add(coordinates);
			}
			if (occupiedBlock.Occupation.Intersects(BlockOccupations.Top))
			{
				_topOccupiedCoordinates.Add(coordinates);
			}
		}
		AddUnblockedEdges(blockObject, blockObjectNavMeshSettingsSpec);
		AddBlockedEdges(blockObject, blockObjectNavMeshSettingsSpec);
		foreach (NavMeshEdge blockedEdge in _blockedEdges)
		{
			if (CanAddBlockingEdge(blockedEdge))
			{
				navMeshObject.BlockEdge(blockedEdge);
			}
		}
		_addedEdges.Clear();
		_bottomOccupiedCoordinates.Clear();
		_topOccupiedCoordinates.Clear();
		_coordinatesBelowEntranceLevel.Clear();
		_blockedEdges.Clear();
		_unblockedEdges.Clear();
	}

	private void AddManuallySetEdges(BlockObject blockObject, BlockObjectNavMeshSettingsSpec blockObjectNavMeshSettingsSpec)
	{
		foreach (BlockObjectNavMeshEdgeGroupSpec edgeGroup in blockObjectNavMeshSettingsSpec.EdgeGroups)
		{
			foreach (BlockObjectNavMeshEdgeSpec addedEdge in edgeGroup.AddedEdges)
			{
				Vector3Int vector3Int = blockObject.TransformCoordinates(addedEdge.Start);
				Vector3Int vector3Int2 = blockObject.TransformCoordinates(addedEdge.End);
				_addedEdges.Add(GetManuallySetEdge(vector3Int, vector3Int2, edgeGroup));
				if (addedEdge.IsTwoWay)
				{
					_addedEdges.Add(GetManuallySetEdge(vector3Int2, vector3Int, edgeGroup));
				}
			}
		}
	}

	private NavMeshEdge GetManuallySetEdge(Vector3Int start, Vector3Int end, BlockObjectNavMeshEdgeGroupSpec edgeGroup)
	{
		int groupId = (edgeGroup.UseGroup ? _navMeshGroupService.GetOrAddGroupId(edgeGroup.GroupName) : _navMeshGroupService.GetDefaultGroupId());
		return NavMeshEdge.CreateGrouped(start, end, groupId, edgeGroup.IsPath, edgeGroup.Cost);
	}

	private void AddEdgesAddedByStackable(BlockObject blockObject, BlockObjectNavMeshSettingsSpec blockObjectNavMeshSettingsSpec)
	{
		if (!blockObjectNavMeshSettingsSpec.GenerateFloorsOnStackable)
		{
			return;
		}
		foreach (Block allBlock in blockObject.PositionedBlocks.GetAllBlocks())
		{
			if (allBlock.Stackable.IsStackable())
			{
				Vector3Int vector3Int = allBlock.Coordinates.Above();
				Vector3Int[] neighbors4Vector3Int = Deltas.Neighbors4Vector3Int;
				foreach (Vector3Int vector3Int2 in neighbors4Vector3Int)
				{
					_addedEdges.Add(NavMeshEdge.CreateDefault(vector3Int, vector3Int + vector3Int2));
				}
			}
		}
	}

	private void AddUnblockedEdges(BlockObject blockObject, BlockObjectNavMeshSettingsSpec blockObjectNavMeshSettingsSpec)
	{
		if (!(blockObjectNavMeshSettingsSpec != null))
		{
			return;
		}
		foreach (BlockObjectNavMeshUnblockedCoordinatesSpec unblockedCoordinate in blockObjectNavMeshSettingsSpec.UnblockedCoordinates)
		{
			Vector3Int vector3Int = blockObject.TransformCoordinates(unblockedCoordinate.Coordinates);
			int orAddGroupId = _navMeshGroupService.GetOrAddGroupId(unblockedCoordinate.Group);
			Vector3Int[] neighbors8Vector3IntOrdered = Deltas.Neighbors8Vector3IntOrdered;
			foreach (Vector3Int vector3Int2 in neighbors8Vector3IntOrdered)
			{
				Vector3Int end = vector3Int + vector3Int2;
				_unblockedEdges.Add(NavMeshEdge.CreateBlocking(vector3Int, end, orAddGroupId));
			}
		}
	}

	private void AddBlockedEdges(BlockObject blockObject, BlockObjectNavMeshSettingsSpec blockObjectNavMeshSettingsSpec)
	{
		foreach (Vector3Int topOccupiedCoordinate in _topOccupiedCoordinates)
		{
			AddBlockedEdges(topOccupiedCoordinate, topOccupiedCoordinate.Above());
		}
		if ((object)blockObjectNavMeshSettingsSpec != null && blockObjectNavMeshSettingsSpec.BlockedEdges.Length > 0)
		{
			foreach (BlockObjectNavMeshBlockedEdgeSpec blockedEdge in blockObjectNavMeshSettingsSpec.BlockedEdges)
			{
				Vector3Int start = blockObject.TransformCoordinates(blockedEdge.Start);
				Vector3Int end = blockObject.TransformCoordinates(blockedEdge.End);
				int orAddGroupId = _navMeshGroupService.GetOrAddGroupId(blockedEdge.Group);
				_blockedEdges.Add(NavMeshEdge.CreateBlocking(start, end, orAddGroupId));
			}
		}
		if ((object)blockObjectNavMeshSettingsSpec == null || !blockObjectNavMeshSettingsSpec.NoAutoWalls)
		{
			NavMeshEdge? entranceEdge = GetEntranceEdge(blockObject);
			if (entranceEdge.HasValue)
			{
				NavMeshEdge valueOrDefault = entranceEdge.GetValueOrDefault();
				AddEdgesBlockedByElevatedEntrance(blockObject);
				AddEdgesBlockedByWalls(_bottomOccupiedCoordinates);
				_blockedEdges.Remove(valueOrDefault);
			}
			else
			{
				AddEdgesBlockedByCoordinates(_bottomOccupiedCoordinates);
			}
		}
	}

	private void AddEdgesBlockedByElevatedEntrance(BlockObject blockObject)
	{
		if (blockObject.Entrance.Coordinates.z <= 0)
		{
			return;
		}
		int z = blockObject.PositionedEntrance.Coordinates.z;
		foreach (Block allBlock in blockObject.PositionedBlocks.GetAllBlocks())
		{
			if (allBlock.Coordinates.z < z && allBlock.Occupation.HasBottomOrFloorOrFull())
			{
				_coordinatesBelowEntranceLevel.Add(allBlock.Coordinates);
			}
		}
		AddEdgesBlockedByCoordinates(_coordinatesBelowEntranceLevel);
	}

	private void AddEdgesBlockedByCoordinates(ICollection<Vector3Int> coordinates)
	{
		foreach (Vector3Int coordinate in coordinates)
		{
			AddBlockedEdgesToNeighbors(coordinate, coordinates);
		}
		AddEdgesBlockedByWalls(coordinates);
	}

	private void AddBlockedEdgesToNeighbors(Vector3Int coordinates, ICollection<Vector3Int> neighbors)
	{
		Vector3Int[] neighbors8Vector3IntOrdered = Deltas.Neighbors8Vector3IntOrdered;
		foreach (Vector3Int vector3Int in neighbors8Vector3IntOrdered)
		{
			Vector3Int vector3Int2 = coordinates + vector3Int;
			if (neighbors.Contains(vector3Int2))
			{
				AddBlockedEdges(coordinates, vector3Int2);
			}
		}
	}

	private void AddEdgesBlockedByWalls(ICollection<Vector3Int> coordinates)
	{
		foreach (Vector3Int coordinate in coordinates)
		{
			AddEdgesBlockedByWall(coordinate, coordinates);
		}
	}

	private NavMeshEdge? GetEntranceEdge(BlockObject blockObject)
	{
		if (blockObject.HasEntrance)
		{
			PositionedEntrance positionedEntrance = blockObject.PositionedEntrance;
			return NavMeshEdge.CreateBlocking(positionedEntrance.DoorstepCoordinates, positionedEntrance.Coordinates, _navMeshGroupService.GetDefaultGroupId());
		}
		return null;
	}

	private void AddEdgesBlockedByWall(Vector3Int coordinates, ICollection<Vector3Int> ignoredNeighbors)
	{
		Vector3Int[] neighbors4Vector3Int = Deltas.Neighbors4Vector3Int;
		foreach (Vector3Int vector3Int in neighbors4Vector3Int)
		{
			if (!ignoredNeighbors.Contains(coordinates + vector3Int))
			{
				AddEdgesBlockedByWall(coordinates, vector3Int);
			}
		}
	}

	private void AddEdgesBlockedByWall(Vector3Int occupiedCoords, Vector3Int neighborDelta)
	{
		Vector3Int vector3Int = occupiedCoords + neighborDelta;
		AddBlockedEdges(occupiedCoords, vector3Int);
		Vector3Int vector3Int2 = new Vector3Int(neighborDelta.y, neighborDelta.x, 0);
		AddBlockedEdges(occupiedCoords, vector3Int + vector3Int2);
		AddBlockedEdges(occupiedCoords, vector3Int - vector3Int2);
		AddBlockedEdges(occupiedCoords + vector3Int2, vector3Int);
		AddBlockedEdges(occupiedCoords - vector3Int2, vector3Int);
	}

	private void AddBlockedEdges(Vector3Int start, Vector3Int end)
	{
		foreach (int allGroupId in _navMeshGroupService.GetAllGroupIds())
		{
			NavMeshEdge item = NavMeshEdge.CreateBlocking(start, end, allGroupId);
			if (!_unblockedEdges.Contains(item))
			{
				_blockedEdges.Add(item);
			}
		}
	}

	private bool CanAddBlockingEdge(NavMeshEdge edge)
	{
		foreach (NavMeshEdge addedEdge in _addedEdges)
		{
			if (addedEdge.Start == edge.Start && addedEdge.End == edge.End && addedEdge.GroupId == edge.GroupId)
			{
				return false;
			}
		}
		return true;
	}
}
