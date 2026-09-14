using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.Navigation;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Buildings;

public class BuildingModelUpdater : ILoadableSingleton, ISingletonPreviewNavMeshListener, ISingletonInstantNavMeshListener
{
	private readonly IBlockService _blockService;

	private readonly EventBus _eventBus;

	private bool _loaded;

	public BuildingModelUpdater(IBlockService blockService, EventBus eventBus)
	{
		_blockService = blockService;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
		_loaded = true;
	}

	public void OnPreviewNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		OnNavMeshUpdated(navMeshUpdate);
	}

	public void OnInstantNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		OnNavMeshUpdated(navMeshUpdate);
	}

	[OnEvent]
	public void OnBlockObjectSetEvent(BlockObjectSetEvent blockObjectSetEvent)
	{
		UpdateBuildingsModelsAround(blockObjectSetEvent.BlockObject);
		UpdateBuildingModelsBelow(blockObjectSetEvent.BlockObject);
	}

	[OnEvent]
	public void OnBlockObjectUnsetEvent(BlockObjectUnsetEvent blockObjectUnsetEvent)
	{
		UpdateBuildingsModelsAround(blockObjectUnsetEvent.BlockObject);
		UpdateBuildingModelsBelow(blockObjectUnsetEvent.BlockObject);
	}

	private void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		UpdateBuildingsModelsAt(navMeshUpdate.TerrainCoordinates);
		UpdateBuildingModelsBelow(navMeshUpdate.TerrainCoordinates);
	}

	private void UpdateBuildingsModelsAt(ReadOnlyList<Vector3Int> coordinates)
	{
		if (_loaded)
		{
			for (int i = 0; i < coordinates.Count; i++)
			{
				UpdateBuildingModelsAt(coordinates[i]);
			}
		}
	}

	private void UpdateBuildingModelsAt(Vector3Int coordinates)
	{
		ReadOnlyList<BlockObject> objectsAt = _blockService.GetObjectsAt(coordinates);
		for (int i = 0; i < objectsAt.Count; i++)
		{
			objectsAt[i].GetComponent<BlockObjectModelController>()?.UpdateModel();
		}
	}

	private void UpdateBuildingsModelsAround(BlockObject blockObject)
	{
		Vector3Int coordinates = blockObject.Coordinates;
		Vector3Int b = coordinates + OrientationExtensions.Transform(vector: blockObject.Blocks.Size - new Vector3Int(1, 1, 1), orientation: blockObject.Orientation);
		(Vector3Int min, Vector3Int max) tuple = Vectors.MinMax(coordinates, b);
		Vector3Int item = tuple.min;
		Vector3Int item2 = tuple.max;
		for (int i = item.z; i <= item2.z; i++)
		{
			for (int j = item.x - 1; j <= item2.x + 1; j++)
			{
				UpdateBuildingModelsAt(new Vector3Int(j, item.y - 1, i));
				UpdateBuildingModelsAt(new Vector3Int(j, item2.y + 1, i));
			}
			for (int k = item.y; k <= item2.y; k++)
			{
				UpdateBuildingModelsAt(new Vector3Int(item.x - 1, k, i));
				UpdateBuildingModelsAt(new Vector3Int(item2.x + 1, k, i));
			}
		}
	}

	private void UpdateBuildingModelsBelow(ReadOnlyList<Vector3Int> coordinates)
	{
		if (_loaded)
		{
			for (int i = 0; i < coordinates.Count; i++)
			{
				UpdateBuildingModelsAt(coordinates[i].Below());
			}
		}
	}

	private void UpdateBuildingModelsBelow(BlockObject blockObject)
	{
		Vector3Int coordinates = blockObject.Coordinates;
		UpdateBuildingModelsAt(new Vector3Int(coordinates.x, coordinates.y, coordinates.z - 1));
	}
}
