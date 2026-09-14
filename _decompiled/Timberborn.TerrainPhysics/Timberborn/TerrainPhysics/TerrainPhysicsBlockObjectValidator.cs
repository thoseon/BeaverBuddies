using Timberborn.BlockSystem;

namespace Timberborn.TerrainPhysics;

internal class TerrainPhysicsBlockObjectValidator : IBlockObjectValidator
{
	private readonly ITerrainPhysicsService _terrainPhysicsService;

	private readonly TerrainPhysicsValidationEnabler _terrainPhysicsValidationEnabler;

	public TerrainPhysicsBlockObjectValidator(ITerrainPhysicsService terrainPhysicsService, TerrainPhysicsValidationEnabler terrainPhysicsValidationEnabler)
	{
		_terrainPhysicsService = terrainPhysicsService;
		_terrainPhysicsValidationEnabler = terrainPhysicsValidationEnabler;
	}

	public bool IsValid(BlockObject blockObject, out string errorMessage)
	{
		errorMessage = null;
		if (blockObject.HasComponent<TerrainPhysicsBlockObjectValidatorSpec>())
		{
			if (_terrainPhysicsValidationEnabler.Enabled)
			{
				return _terrainPhysicsService.ValidateBlockObjectPreview(blockObject);
			}
			return true;
		}
		return true;
	}
}
