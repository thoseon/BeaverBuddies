using Timberborn.BlockSystem;
using Timberborn.NaturalResources;
using UnityEngine;

namespace Timberborn.Planting;

public class PlantingAreaValidator
{
	private readonly IBlockService _blockService;

	private readonly SpawnValidationService _spawnValidationService;

	public PlantingAreaValidator(IBlockService blockService, SpawnValidationService spawnValidationService)
	{
		_blockService = blockService;
		_spawnValidationService = spawnValidationService;
	}

	public bool CanPlant(Vector3Int coordinates, string name)
	{
		if (!IsSamePlantable(coordinates, name))
		{
			return _spawnValidationService.IsUnobstructed(coordinates, name);
		}
		return true;
	}

	private bool IsSamePlantable(Vector3Int coordinates, string name)
	{
		PlantableSpec bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<PlantableSpec>(coordinates);
		if ((object)bottomObjectComponentAt != null)
		{
			return bottomObjectComponentAt.TemplateName == name;
		}
		return false;
	}
}
