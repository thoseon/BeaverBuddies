using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;

namespace Timberborn.TerrainPhysics;

internal class TerrainPhysicsDeletionBlocker : BaseComponent, IAwakableComponent, IBlockObjectDeletionBlocker
{
	private readonly ITerrainPhysicsService _terrainPhysicsService;

	private BlockObject _blockObject;

	public bool NoForcedDelete => true;

	public bool IsStackedDeletionBlocked => false;

	public bool IsDeletionBlocked => !_terrainPhysicsService.CanBeDestroyed(_blockObject);

	public string ReasonLocKey => "DeletionBlocker.ObjectAtop";

	public TerrainPhysicsDeletionBlocker(ITerrainPhysicsService terrainPhysicsService)
	{
		_terrainPhysicsService = terrainPhysicsService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}
}
