using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.Navigation;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.BlockObjectAccesses;

public class BlockObjectAccessGenerator : BaseComponent, IAwakableComponent
{
	private readonly struct AccessWithParentedNeighbor
	{
		public Vector3Int Access { get; }

		public ParentedNeighbor2D ParentedNeighbor { get; }

		public AccessWithParentedNeighbor(Vector3Int access, ParentedNeighbor2D parentedNeighbor)
		{
			Access = access;
			ParentedNeighbor = parentedNeighbor;
		}
	}

	private static readonly float DiagonalOffsetScale = 0.3f;

	private static readonly float StraightOffsetScale = 0.2f;

	private readonly INavMeshService _navMeshService;

	private readonly IBlockService _blockService;

	private readonly ITerrainService _terrainService;

	private readonly HashSet<Vector3Int> _visitedValidAccesses = new HashSet<Vector3Int>();

	private readonly Dictionary<Vector2Int, int> _terrainHeightCache = new Dictionary<Vector2Int, int>();

	private BlockObject _blockObject;

	private ParentedNeighborCalculator _parentedNeighborCalculator;

	private bool _isFullyUnderground;

	public BlockObjectAccessGenerator(INavMeshService navMeshService, IBlockService blockService, ITerrainService terrainService)
	{
		_navMeshService = navMeshService;
		_blockService = blockService;
		_terrainService = terrainService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_parentedNeighborCalculator = GetComponent<ParentedNeighborCalculator>();
		_isFullyUnderground = _blockObject.Blocks.GetAllBlocks().All((Block block) => block.Underground);
	}

	public BoundingBox GenerateAccessBounds(int minLevel, int maxLevel)
	{
		BoundingBox.Builder builder = default(BoundingBox.Builder);
		foreach (ParentedNeighbor2D nonInternalParentedNeighbor in _parentedNeighborCalculator.GetNonInternalParentedNeighbors())
		{
			Vector2Int neighbor = nonInternalParentedNeighbor.Neighbor;
			builder.Expand(new Vector3Int(neighbor.x, neighbor.y, minLevel));
			builder.Expand(new Vector3Int(neighbor.x, neighbor.y, maxLevel));
		}
		return builder.Build();
	}

	public IEnumerable<Vector3> GenerateAccesses(int minLevel, int maxLevel)
	{
		UpdateFoundationToTerrainDistances();
		return GetValidAccessWithParentedNeighbor(minLevel, maxLevel).Concat(GetInternalParentedNeighbors(maxLevel)).Select(GetWorldSpaceAccess).Concat(GetBottomAccesses());
	}

	private IEnumerable<AccessWithParentedNeighbor> GetValidAccessWithParentedNeighbor(int minLevel, int maxLevel)
	{
		int topLevel = _blockObject.Coordinates.z + _blockObject.Blocks.Size.z;
		foreach (ParentedNeighbor2D neighbor2D in _parentedNeighborCalculator.GetNonInternalParentedNeighbors())
		{
			Vector2Int accessColumn = neighbor2D.Neighbor;
			Vector2Int parentColumn = neighbor2D.Parent;
			int z = minLevel;
			while (z <= maxLevel)
			{
				Vector3Int vector3Int = accessColumn.ToVector3Int(z);
				if (!NoTerrainInTheWay(parentColumn, vector3Int) && (!_isFullyUnderground || z > topLevel))
				{
					break;
				}
				if (!_visitedValidAccesses.Contains(vector3Int) && AccessIsValid(vector3Int))
				{
					_visitedValidAccesses.Add(vector3Int);
					yield return new AccessWithParentedNeighbor(vector3Int, neighbor2D);
				}
				int num = z + 1;
				z = num;
			}
		}
		_visitedValidAccesses.Clear();
	}

	private IEnumerable<AccessWithParentedNeighbor> GetInternalParentedNeighbors(int maxLevel)
	{
		foreach (Vector2Int column in _terrainHeightCache.Keys)
		{
			int z = _blockObject.CoordinatesAtBaseZ.z;
			while (z <= maxLevel)
			{
				Vector3Int vector3Int = column.ToVector3Int(z);
				if (!_visitedValidAccesses.Contains(vector3Int) && AccessIsValid(vector3Int) && TryGetAccessWithParentedNeighbour(vector3Int, out var accessWithNeighbor))
				{
					yield return accessWithNeighbor;
				}
				int num = z + 1;
				z = num;
			}
		}
		_visitedValidAccesses.Clear();
		_terrainHeightCache.Clear();
	}

	private bool TryGetAccessWithParentedNeighbour(Vector3Int access, out AccessWithParentedNeighbor accessWithNeighbor)
	{
		Vector3Int[] neighbors8Vector3IntOrdered = Deltas.Neighbors8Vector3IntOrdered;
		foreach (Vector3Int vector3Int in neighbors8Vector3IntOrdered)
		{
			Vector3Int vector3Int2 = access + vector3Int;
			if (NoTerrainInTheWay(vector3Int2.XY(), access))
			{
				accessWithNeighbor = new AccessWithParentedNeighbor(access, ParentedNeighbor2D.FromVectors(access, vector3Int2));
				return true;
			}
		}
		accessWithNeighbor = default(AccessWithParentedNeighbor);
		return false;
	}

	private void UpdateFoundationToTerrainDistances()
	{
		int z = _blockObject.Coordinates.z + _blockObject.Blocks.Size.z;
		foreach (Vector3Int occupiedCoordinate in _blockObject.PositionedBlocks.GetOccupiedCoordinates())
		{
			if (occupiedCoordinate.z == _blockObject.CoordinatesAtBaseZ.z)
			{
				Vector3Int vector3Int = (_isFullyUnderground ? new Vector3Int(occupiedCoordinate.x, occupiedCoordinate.y, z) : occupiedCoordinate);
				if (_terrainService.TryGetDistanceToTerrainAbove(vector3Int, out var distance))
				{
					_terrainHeightCache[vector3Int.XY()] = vector3Int.z + distance;
				}
				else
				{
					_terrainHeightCache[vector3Int.XY()] = int.MaxValue;
				}
			}
		}
	}

	private Vector3 GetWorldSpaceAccess(AccessWithParentedNeighbor accessWithParentedNeighbor)
	{
		Vector3Int access = accessWithParentedNeighbor.Access;
		Vector2Int neighbor = accessWithParentedNeighbor.ParentedNeighbor.Neighbor;
		Vector2Int parent = accessWithParentedNeighbor.ParentedNeighbor.Parent;
		Vector3 accessWithOffset = OffsetTowards(neighbor, parent).ToVector3(access.z);
		return GetAccessWithPathAdjustedHeight(access, accessWithOffset);
	}

	private IEnumerable<Vector3> GetBottomAccesses()
	{
		foreach (Block allBlock in _blockObject.PositionedBlocks.GetAllBlocks())
		{
			if (allBlock.IsFoundationBlock || allBlock.Stackable == BlockStackable.UnfinishedGround)
			{
				Vector3Int vector3Int = allBlock.Coordinates.Below();
				if (AccessIsValid(vector3Int))
				{
					yield return CoordinateSystem.GridToWorldCentered(vector3Int);
				}
			}
		}
	}

	private bool NoTerrainInTheWay(Vector2Int parentColumn, Vector3Int access)
	{
		if (_terrainHeightCache.TryGetValue(parentColumn, out var value))
		{
			return access.z < value;
		}
		return false;
	}

	private bool AccessIsValid(Vector3Int access)
	{
		if (!IsAccessBlockedBySelf(access) && _navMeshService.IsOnNavMesh(access))
		{
			BlockObject bottomObjectAt = _blockService.GetBottomObjectAt(access);
			if ((bool)bottomObjectAt)
			{
				return CanPlaceAccessInsideObject(access, bottomObjectAt);
			}
			return true;
		}
		return false;
	}

	private bool IsAccessBlockedBySelf(Vector3Int access)
	{
		if (_blockObject.PositionedBlocks.TryGetBlock(access, out var result) && result.IsOccupied)
		{
			return access.z == _blockObject.CoordinatesAtBaseZ.z;
		}
		return false;
	}

	private static Vector2 OffsetTowards(Vector2Int coordinates, Vector2Int target)
	{
		Vector2Int vector2Int = coordinates - target;
		float num = (OffsetIsDiagonal(vector2Int) ? DiagonalOffsetScale : StraightOffsetScale);
		Vector2 vector = ((Vector2)vector2Int).normalized * num;
		return coordinates - vector;
	}

	private Vector3 GetAccessWithPathAdjustedHeight(Vector3Int access, Vector3 accessWithOffset)
	{
		Vector3 vector = CoordinateSystem.GridToWorldCentered(accessWithOffset);
		IPathHeightProvider bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<IPathHeightProvider>(access);
		if (bottomObjectComponentAt != null && bottomObjectComponentAt.TryGetHeight(vector, out var pathHeight))
		{
			vector.y = ((pathHeight < vector.y) ? pathHeight : vector.y);
		}
		return vector;
	}

	private static bool CanPlaceAccessInsideObject(Vector3Int access, BlockObject blockObject)
	{
		BlockObjectAccesses component = blockObject.GetComponent<BlockObjectAccesses>();
		if (component != null)
		{
			if (component.IsBlocked(access))
			{
				return false;
			}
			if (component.IsAllowed(access))
			{
				return true;
			}
		}
		if (blockObject.Entrance.HasEntrance)
		{
			return blockObject.IsUnfinished;
		}
		return true;
	}

	private static bool OffsetIsDiagonal(Vector2Int offset)
	{
		if (offset.x != 0)
		{
			return offset.y != 0;
		}
		return false;
	}
}
