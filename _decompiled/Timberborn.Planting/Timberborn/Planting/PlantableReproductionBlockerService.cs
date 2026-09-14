using Timberborn.BlockSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Planting;

internal class PlantableReproductionBlockerService : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly IBlockService _blockService;

	public PlantableReproductionBlockerService(EventBus eventBus, IBlockService blockService)
	{
		_eventBus = eventBus;
		_blockService = blockService;
	}

	[OnEvent]
	public void OnPlantingCoordinatesSet(PlantingCoordinatesSetEvent plantingCoordinatesSetEvent)
	{
		BlockReproductionAt(plantingCoordinatesSetEvent.Coordinates);
	}

	[OnEvent]
	public void OnPlantingCoordinatesUnset(PlantingCoordinatesUnsetEvent plantingCoordinatesUnsetEvent)
	{
		UnblockReproductionAt(plantingCoordinatesUnsetEvent.Coordinates);
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	private void BlockReproductionAt(Vector3Int coordinates)
	{
		_blockService.GetBottomObjectComponentAt<PlantableReproductionBlocker>(coordinates)?.BlockReproduction();
	}

	private void UnblockReproductionAt(Vector3Int coordinates)
	{
		_blockService.GetBottomObjectComponentAt<PlantableReproductionBlocker>(coordinates)?.UnblockReproduction();
	}
}
