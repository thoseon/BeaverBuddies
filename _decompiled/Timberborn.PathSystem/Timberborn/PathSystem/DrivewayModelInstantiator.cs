using System;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Buildings;
using Timberborn.Coordinates;
using Timberborn.GameFactionSystem;
using Timberborn.PrefabOptimization;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.PathSystem;

internal class DrivewayModelInstantiator : ILoadableSingleton
{
	private readonly FactionService _factionService;

	private readonly OptimizedPrefabInstantiator _optimizedPrefabInstantiator;

	private readonly ISpecService _specService;

	private GameObject _narrowLeftDrivewayPrefab;

	private GameObject _narrowCenterDrivewayPrefab;

	private GameObject _narrowRightDrivewayPrefab;

	private GameObject _wideCenterDrivewayPrefab;

	private GameObject _longCenterDrivewayPrefab;

	private GameObject _straightPathDrivewayPrefab;

	private Material _pathMaterial;

	public DrivewayModelInstantiator(FactionService factionService, OptimizedPrefabInstantiator optimizedPrefabInstantiator, ISpecService specService)
	{
		_factionService = factionService;
		_optimizedPrefabInstantiator = optimizedPrefabInstantiator;
		_specService = specService;
	}

	public void Load()
	{
		DrivewayModelInstantiatorSpec singleSpec = _specService.GetSingleSpec<DrivewayModelInstantiatorSpec>();
		_narrowLeftDrivewayPrefab = singleSpec.NarrowLeftDrivewayPrefab.Asset;
		_narrowCenterDrivewayPrefab = singleSpec.NarrowCenterDrivewayPrefab.Asset;
		_narrowRightDrivewayPrefab = singleSpec.NarrowRightDrivewayPrefab.Asset;
		_wideCenterDrivewayPrefab = singleSpec.WideCenterDrivewayPrefab.Asset;
		_longCenterDrivewayPrefab = singleSpec.LongCenterDrivewayPrefab.Asset;
		_straightPathDrivewayPrefab = singleSpec.StraightPathDrivewayPrefab.Asset;
		_pathMaterial = _factionService.Current.PathMaterial.Asset;
	}

	internal GameObject InstantiateModel(DrivewayModel drivewayModel, BlockObject blockObject, Vector3Int coordinates, Direction2D direction)
	{
		GameObject modelPrefab = GetModelPrefab(drivewayModel.Driveway);
		Transform transform = blockObject.GetComponent<BuildingModel>().FinishedModel.transform;
		GameObject gameObject = _optimizedPrefabInstantiator.Instantiate(modelPrefab, transform);
		Vector3 localPosition = CoordinateSystem.GridToWorld(blockObject.Blocks.Pivot(coordinates, direction.ToOrientation()));
		Quaternion localRotation = direction.ToWorldSpaceRotation();
		gameObject.transform.SetLocalPositionAndRotation(localPosition, localRotation);
		gameObject.GetComponentInChildren<Renderer>().sharedMaterial = _pathMaterial;
		return gameObject;
	}

	private GameObject GetModelPrefab(Driveway driveway)
	{
		return driveway switch
		{
			Driveway.NarrowLeft => _narrowLeftDrivewayPrefab, 
			Driveway.NarrowCenter => _narrowCenterDrivewayPrefab, 
			Driveway.NarrowRight => _narrowRightDrivewayPrefab, 
			Driveway.WideCenter => _wideCenterDrivewayPrefab, 
			Driveway.LongCenter => _longCenterDrivewayPrefab, 
			Driveway.StraightPath => _straightPathDrivewayPrefab, 
			_ => throw new ArgumentOutOfRangeException("driveway", driveway, null), 
		};
	}
}
