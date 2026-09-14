using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockSystemNavigation;
using Timberborn.TerrainSystem;

namespace Timberborn.BlockObstacles;

public class BlockOccupier : BaseComponent, IAwakableComponent
{
	private readonly ITerrainService _terrainService;

	private readonly IBlockService _blockService;

	private IBlockObjectNavMesh _blockObjectNavMesh;

	private bool _isAddedToServices;

	private readonly List<BlockObject> _blockObjectCache = new List<BlockObject>();

	public BlockObject BlockObject { get; private set; }

	public BlockOccupier(ITerrainService terrainService, IBlockService blockService)
	{
		_terrainService = terrainService;
		_blockService = blockService;
	}

	public void Awake()
	{
		BlockObject = GetComponent<BlockObject>();
		_blockObjectNavMesh = GetComponent<IBlockObjectNavMesh>();
	}

	public bool CanBeAddedToServices()
	{
		if (!IsUnderground())
		{
			return CoordinatesHaveNoOtherObjectsExceptFloor();
		}
		return false;
	}

	public void AddToServices()
	{
		if (!_isAddedToServices)
		{
			BlockObject.MarkAsFinishedAndAddToServices();
			_blockObjectNavMesh.RecalculateNavMeshObject();
			_blockObjectNavMesh.NavMeshObject.EnqueueAddToRegularNavMesh();
			_isAddedToServices = true;
		}
	}

	public void RemoveFromServices()
	{
		if (_isAddedToServices)
		{
			BlockObject.MarkAsPreview();
			_blockObjectNavMesh.NavMeshObject.EnqueueRemoveFromRegularNavMesh();
			_isAddedToServices = false;
		}
	}

	private bool IsUnderground()
	{
		return _terrainService.Underground(BlockObject.Coordinates);
	}

	private bool CoordinatesHaveNoOtherObjectsExceptFloor()
	{
		_blockService.GetIntersectingObjectsAt(BlockObject.Coordinates, ~BlockOccupations.Floor, _blockObjectCache);
		bool result = HasNoOtherObject();
		_blockObjectCache.Clear();
		return result;
	}

	private bool HasNoOtherObject()
	{
		foreach (BlockObject item in _blockObjectCache)
		{
			if (item != BlockObject && !item.Overridable)
			{
				return false;
			}
		}
		return true;
	}
}
