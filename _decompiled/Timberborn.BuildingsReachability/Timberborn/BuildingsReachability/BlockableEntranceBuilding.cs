using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Navigation;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.BuildingsReachability;

public class BlockableEntranceBuilding : BaseComponent, IAwakableComponent
{
	private readonly IBlockService _blockService;

	private readonly PreviewBlockService _previewBlockService;

	private readonly ITerrainService _terrainService;

	private readonly INavMeshService _navMeshService;

	private BlockObject _blockObject;

	private Vector3Int EntranceCoordinates => _blockObject.PositionedEntrance.Coordinates;

	public BlockableEntranceBuilding(IBlockService blockService, PreviewBlockService previewBlockService, ITerrainService terrainService, INavMeshService navMeshService)
	{
		_blockService = blockService;
		_previewBlockService = previewBlockService;
		_terrainService = terrainService;
		_navMeshService = navMeshService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public bool IsEntranceBlocked()
	{
		if (!_blockObject.HasEntrance)
		{
			return false;
		}
		if (IsBlockedByTerrain())
		{
			return true;
		}
		if (IsBlockedByNavMesh())
		{
			return IsBlockedByOtherObject();
		}
		return false;
	}

	public bool IsEntranceBlockedByCoordinates(IEnumerable<Vector3Int> coordinates)
	{
		if (IsEntranceBlocked())
		{
			return coordinates.Contains(EntranceCoordinates);
		}
		return false;
	}

	public bool IsEntranceInaccessible()
	{
		if (!_blockObject.HasEntrance)
		{
			return false;
		}
		if (!IsBlockedByTerrain())
		{
			return IsBlockedByNavMesh();
		}
		return true;
	}

	private bool IsBlockedByTerrain()
	{
		return _terrainService.Underground(EntranceCoordinates);
	}

	private bool IsBlockedByNavMesh()
	{
		Vector3Int doorstepCoordinates = _blockObject.PositionedEntrance.DoorstepCoordinates;
		return !_navMeshService.AreConnectedPreview(doorstepCoordinates, EntranceCoordinates);
	}

	private bool IsBlockedByOtherObject()
	{
		BlockObject bottomPreviewAt = _previewBlockService.GetBottomPreviewAt(EntranceCoordinates);
		if (bottomPreviewAt == null || bottomPreviewAt.Overridable)
		{
			bottomPreviewAt = _blockService.GetBottomObjectAt(EntranceCoordinates);
			if (bottomPreviewAt != null)
			{
				return !bottomPreviewAt.Overridable;
			}
			return false;
		}
		return true;
	}
}
