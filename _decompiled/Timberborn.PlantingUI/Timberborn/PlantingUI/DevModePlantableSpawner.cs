using System.Collections.Generic;
using Timberborn.Gathering;
using Timberborn.Growing;
using Timberborn.InputSystem;
using Timberborn.NaturalResources;
using UnityEngine;

namespace Timberborn.PlantingUI;

public class DevModePlantableSpawner
{
	private static readonly string PlantSpawnedKey = "PlantSpawned";

	private static readonly string PlantGrownKey = "PlantGrown";

	private static readonly string PlantWithYieldKey = "PlantWithYield";

	private readonly InputService _inputService;

	private readonly NaturalResourceFactory _naturalResourceFactory;

	public DevModePlantableSpawner(InputService inputService, NaturalResourceFactory naturalResourceFactory)
	{
		_inputService = inputService;
		_naturalResourceFactory = naturalResourceFactory;
	}

	public void SpawnPlantables(IEnumerable<Vector3Int> blocks, string resourceId)
	{
		if (!_inputService.IsKeyHeld(PlantSpawnedKey))
		{
			return;
		}
		foreach (Vector3Int block in blocks)
		{
			Vector3Int coordinates = block + new Vector3Int(0, 0, 1);
			NaturalResource naturalResource = _naturalResourceFactory.SpawnIgnoringConstraints(resourceId, coordinates);
			if ((bool)naturalResource && _inputService.IsKeyHeld(PlantGrownKey))
			{
				naturalResource.GetComponent<Growable>().IncreaseGrowthProgress(1f);
				if (_inputService.IsKeyHeld(PlantWithYieldKey))
				{
					naturalResource.GetComponent<GatherableYieldGrower>()?.FastForwardGrowth(1f);
				}
			}
		}
	}
}
