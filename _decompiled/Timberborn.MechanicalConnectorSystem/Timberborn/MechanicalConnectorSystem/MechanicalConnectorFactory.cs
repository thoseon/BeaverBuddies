using Timberborn.AssetSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Coordinates;
using Timberborn.MechanicalSystem;
using Timberborn.PrefabOptimization;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.MechanicalConnectorSystem;

internal class MechanicalConnectorFactory : ILoadableSingleton
{
	private readonly OptimizedPrefabInstantiator _optimizedPrefabInstantiator;

	private readonly IAssetLoader _assetLoader;

	private readonly ISpecService _specService;

	private GameObject _mechanicalConnectorPrefab;

	public MechanicalConnectorFactory(OptimizedPrefabInstantiator optimizedPrefabInstantiator, IAssetLoader assetLoader, ISpecService specService)
	{
		_optimizedPrefabInstantiator = optimizedPrefabInstantiator;
		_assetLoader = assetLoader;
		_specService = specService;
	}

	public void Load()
	{
		MechanicalConnectorFactorySpec singleSpec = _specService.GetSingleSpec<MechanicalConnectorFactorySpec>();
		_mechanicalConnectorPrefab = _assetLoader.Load<GameObject>(singleSpec.MechanicalConnectorPrefabPath);
	}

	public void Create(Transput transput, Transform parent, MechanicalConnectors mechanicalConnectors)
	{
		GameObject gameObject = _optimizedPrefabInstantiator.Instantiate(_mechanicalConnectorPrefab, parent);
		Vector3 localPosition = CoordinateSystem.GridToWorld(transput.Offset) + new Vector3(0.5f, 0.5f, 0.5f);
		Quaternion localRotation = transput.BaseDirection.ToRotation();
		gameObject.transform.SetLocalPositionAndRotation(localPosition, localRotation);
		mechanicalConnectors.Add(transput, gameObject);
	}
}
