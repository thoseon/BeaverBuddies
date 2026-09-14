using Timberborn.BlockSystem;
using Timberborn.TerrainSystem;

namespace Timberborn.TerrainPhysics;

internal class TerrainPhysicsValidatorFactory
{
	private readonly ITerrainService _terrainService;

	private readonly StackableBlockService _stackableBlockService;

	private readonly PreviewBlockService _previewBlockService;

	private readonly SupportsToBeDeleted _supportsToBeDeleted;

	public TerrainPhysicsValidatorFactory(ITerrainService terrainService, StackableBlockService stackableBlockService, PreviewBlockService previewBlockService, SupportsToBeDeleted supportsToBeDeleted)
	{
		_terrainService = terrainService;
		_stackableBlockService = stackableBlockService;
		_previewBlockService = previewBlockService;
		_supportsToBeDeleted = supportsToBeDeleted;
	}

	public TerrainPhysicsValidator CreateValidator()
	{
		TerrainPhysicsValidator terrainPhysicsValidator = new TerrainPhysicsValidator(_terrainService, _stackableBlockService, _previewBlockService, _supportsToBeDeleted, validatePreviewBlocks: false);
		terrainPhysicsValidator.Initialize();
		return terrainPhysicsValidator;
	}

	public TerrainPhysicsValidator CreatePreviewValidator()
	{
		TerrainPhysicsValidator terrainPhysicsValidator = new TerrainPhysicsValidator(_terrainService, _stackableBlockService, _previewBlockService, _supportsToBeDeleted, validatePreviewBlocks: true);
		terrainPhysicsValidator.Initialize();
		return terrainPhysicsValidator;
	}
}
