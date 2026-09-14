using Timberborn.AssetSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.PrefabOptimization;
using Timberborn.Rendering;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.BuildingDoorsteps;

internal class BuildingDoorstepSpawner : ILoadableSingleton
{
	private static readonly Vector3 DoorstepModelOffset = Vector3.up;

	private readonly OptimizedPrefabInstantiator _optimizedPrefabInstantiator;

	private readonly EventBus _eventBus;

	private readonly IAssetLoader _assetLoader;

	private GameObject _doorstepPrefab;

	public BuildingDoorstepSpawner(OptimizedPrefabInstantiator optimizedPrefabInstantiator, EventBus eventBus, IAssetLoader assetLoader)
	{
		_optimizedPrefabInstantiator = optimizedPrefabInstantiator;
		_eventBus = eventBus;
		_assetLoader = assetLoader;
	}

	public void Load()
	{
		_doorstepPrefab = _assetLoader.Load<GameObject>("ConstructionBases/Doorstep/Doorstep.Model");
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnEntityInitialized(EntityInitializedEvent entityInitializedEvent)
	{
		BuildingModel component = entityInitializedEvent.Entity.GetComponent<BuildingModel>();
		if ((bool)component)
		{
			SpawnDoorstep(component);
		}
	}

	private void SpawnDoorstep(BuildingModel buildingModel)
	{
		BlockObject component = buildingModel.GetComponent<BlockObject>();
		if (CanSpawnDoorstep(component))
		{
			GameObject gameObject = _optimizedPrefabInstantiator.Instantiate(_doorstepPrefab, buildingModel.UnfinishedModel.transform);
			Vector3 coordinates = component.Entrance.Coordinates + DoorstepModelOffset - new Vector3Int(0, 0, component.BaseZ);
			gameObject.transform.localPosition = CoordinateSystem.GridToWorld(coordinates);
			buildingModel.GetComponent<BlockObjectModelController>().UpdateAll();
			buildingModel.GetComponent<EntityMaterials>()?.AddMaterials(gameObject);
		}
	}

	private static bool CanSpawnDoorstep(BlockObject blockObject)
	{
		if (blockObject.HasEntrance && blockObject.Entrance.Coordinates.z - blockObject.BaseZ == 0)
		{
			return !blockObject.HasComponent<DoorstepSpawnDisablerSpec>();
		}
		return false;
	}
}
