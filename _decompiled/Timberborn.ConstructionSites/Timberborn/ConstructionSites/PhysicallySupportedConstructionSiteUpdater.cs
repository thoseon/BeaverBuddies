using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.TerrainPhysics;
using UnityEngine;

namespace Timberborn.ConstructionSites;

public class PhysicallySupportedConstructionSiteUpdater : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly IBlockService _blockService;

	private readonly ITerrainPhysicsService _terrainPhysicsService;

	private BlockObject _blockObject;

	public PhysicallySupportedConstructionSiteUpdater(IBlockService blockService, ITerrainPhysicsService terrainPhysicsService)
	{
		_blockService = blockService;
		_terrainPhysicsService = terrainPhysicsService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public void OnEnterFinishedState()
	{
		UpdateNeighbours();
	}

	public void OnExitFinishedState()
	{
	}

	public void UpdateNeighbours()
	{
		if (!_blockObject.Solid)
		{
			return;
		}
		foreach (Block allBlock in _blockObject.PositionedBlocks.GetAllBlocks())
		{
			if (!allBlock.Stackable.IsStackable())
			{
				continue;
			}
			Vector3Int coordinates = allBlock.Coordinates;
			foreach (Vector3Int physicsSupportDelta in _terrainPhysicsService.PhysicsSupportDeltas)
			{
				UpdateNeighbour(coordinates + physicsSupportDelta);
			}
		}
	}

	private void UpdateNeighbour(Vector3Int coordinates)
	{
		foreach (PhysicallySupportedConstructionSite item in _blockService.GetObjectsWithComponentAt<PhysicallySupportedConstructionSite>(coordinates))
		{
			item.Validate();
		}
	}
}
