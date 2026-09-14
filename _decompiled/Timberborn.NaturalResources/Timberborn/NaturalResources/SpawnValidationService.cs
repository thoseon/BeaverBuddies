using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.SoilMoistureSystem;
using Timberborn.TemplateSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.NaturalResources;

public class SpawnValidationService
{
	private readonly ITerrainService _terrainService;

	private readonly BlockValidator _blockValidator;

	private readonly ISoilMoistureService _soilMoistureService;

	private readonly TemplateNameMapper _templateNameMapper;

	private readonly ImmutableArray<ISpawnValidator> _spawnValidators;

	public SpawnValidationService(ITerrainService terrainService, BlockValidator blockValidator, ISoilMoistureService soilMoistureService, TemplateNameMapper templateNameMapper, IEnumerable<ISpawnValidator> spawnValidators)
	{
		_terrainService = terrainService;
		_blockValidator = blockValidator;
		_soilMoistureService = soilMoistureService;
		_templateNameMapper = templateNameMapper;
		_spawnValidators = spawnValidators.ToImmutableArray();
	}

	public bool CanSpawnIgnoringConstraints(Vector3Int coordinates, BlockObjectSpec blockObjectSpec)
	{
		if (_terrainService.OnGround(coordinates))
		{
			return IsUnobstructed(coordinates, blockObjectSpec);
		}
		return false;
	}

	public bool CanSpawn(Vector3Int coordinates, BlockObjectSpec blockObjectSpec, string resourceId)
	{
		if (IsSuitableTerrain(coordinates) && SpotIsValid(coordinates, resourceId))
		{
			return IsUnobstructed(coordinates, blockObjectSpec);
		}
		return false;
	}

	public bool IsUnobstructed(Vector3Int coordinates, string resourceId)
	{
		BlockObjectSpec spec = _templateNameMapper.GetTemplate(resourceId).GetSpec<BlockObjectSpec>();
		return IsUnobstructed(coordinates, spec);
	}

	private bool IsSuitableTerrain(Vector3Int coordinates)
	{
		if (_terrainService.OnGround(coordinates) && _soilMoistureService.SoilIsMoist(coordinates))
		{
			return !_terrainService.CellIsField(coordinates);
		}
		return false;
	}

	private bool SpotIsValid(Vector3Int coordinates, string resourceId)
	{
		for (int i = 0; i < _spawnValidators.Length; i++)
		{
			if (!_spawnValidators[i].CanSpawn(coordinates, resourceId))
			{
				return false;
			}
		}
		return true;
	}

	private bool IsUnobstructed(Vector3Int coordinates, BlockObjectSpec blockObjectSpec)
	{
		return _blockValidator.BlocksValid(blockObjectSpec, new Placement(coordinates));
	}
}
