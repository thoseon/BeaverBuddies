using Timberborn.NaturalResources;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.NaturalResourcesMoisture;

internal class WateredNaturalResourceSpawnValidator : ISpawnValidator
{
	private readonly FloodableNaturalResourceService _floodableNaturalResourceService;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	public WateredNaturalResourceSpawnValidator(FloodableNaturalResourceService floodableNaturalResourceService, IThreadSafeWaterMap threadSafeWaterMap)
	{
		_floodableNaturalResourceService = floodableNaturalResourceService;
		_threadSafeWaterMap = threadSafeWaterMap;
	}

	public bool CanSpawn(Vector3Int coordinates, string resourceTemplateName)
	{
		if (_floodableNaturalResourceService.IsFloodableNaturalResource(resourceTemplateName))
		{
			return _floodableNaturalResourceService.ConditionsAreMet(resourceTemplateName, coordinates);
		}
		return !_threadSafeWaterMap.CellIsUnderwater(coordinates);
	}
}
