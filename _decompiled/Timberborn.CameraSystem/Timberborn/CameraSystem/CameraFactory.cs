using Bindito.Unity;
using Timberborn.AssetSystem;
using Timberborn.RootProviders;
using UnityEngine;

namespace Timberborn.CameraSystem;

public class CameraFactory
{
	private static readonly string CameraPrefabPath = "Camera/Camera";

	private readonly IAssetLoader _assetLoader;

	private readonly IInstantiator _instantiator;

	private readonly RootObjectProvider _rootObjectProvider;

	public CameraFactory(IAssetLoader assetLoader, IInstantiator instantiator, RootObjectProvider rootObjectProvider)
	{
		_assetLoader = assetLoader;
		_instantiator = instantiator;
		_rootObjectProvider = rootObjectProvider;
	}

	public Camera Create(string name)
	{
		GameObject prefab = _assetLoader.Load<GameObject>(CameraPrefabPath);
		GameObject gameObject = _rootObjectProvider.CreateRootObject(name);
		return _instantiator.Instantiate(prefab, gameObject.transform).GetComponent<Camera>();
	}
}
