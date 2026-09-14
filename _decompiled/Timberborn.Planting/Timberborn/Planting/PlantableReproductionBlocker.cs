using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.NaturalResourcesReproduction;
using UnityEngine;

namespace Timberborn.Planting;

internal class PlantableReproductionBlocker : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private readonly PlantingService _plantingService;

	private Reproducible _reproducible;

	public PlantableReproductionBlocker(PlantingService plantingService)
	{
		_plantingService = plantingService;
	}

	public void Awake()
	{
		_reproducible = GetComponent<Reproducible>();
	}

	public void InitializeEntity()
	{
		Vector3Int coordinates = GetComponent<BlockObject>().Coordinates;
		if (_plantingService.IsResourceAt(coordinates))
		{
			BlockReproduction();
		}
	}

	public void BlockReproduction()
	{
		_reproducible.BlockReproduction(this);
	}

	public void UnblockReproduction()
	{
		_reproducible.UnblockReproduction(this);
	}
}
