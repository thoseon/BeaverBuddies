using Timberborn.AssetSystem;
using Timberborn.Debugging;
using Timberborn.PrefabOptimization;
using Timberborn.Rendering;
using Timberborn.SingletonSystem;
using Timberborn.ZiplineSystem;
using UnityEngine;

namespace Timberborn.ZiplineSystemUI;

internal class ZiplineConnectionDevModule : IDevModule, ILoadableSingleton, IUpdatableSingleton
{
	private static readonly string PrefabPath = "UI/Markers/Debug/Tile";

	private readonly ZiplineConnectionService _ziplineConnectionService;

	private readonly MeshDrawerFactory _markerDrawerFactory;

	private readonly IPrefabOptimizationChain _prefabOptimizationChain;

	private readonly IAssetLoader _assetLoader;

	private MeshDrawer _meshDrawer;

	private bool _enabled;

	public ZiplineConnectionDevModule(ZiplineConnectionService ziplineConnectionService, MeshDrawerFactory markerDrawerFactory, IPrefabOptimizationChain prefabOptimizationChain, IAssetLoader assetLoader)
	{
		_ziplineConnectionService = ziplineConnectionService;
		_markerDrawerFactory = markerDrawerFactory;
		_prefabOptimizationChain = prefabOptimizationChain;
		_assetLoader = assetLoader;
	}

	public void Load()
	{
		GameObject inputPrefab = _assetLoader.Load<GameObject>(PrefabPath);
		GameObject gameObject = _prefabOptimizationChain.Process(inputPrefab);
		_meshDrawer = _markerDrawerFactory.Create(gameObject.GetComponent<MeshFilter>().sharedMesh, gameObject.GetComponent<MeshRenderer>().sharedMaterial);
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Toggle zipline cable blocks", delegate
		{
			_enabled = !_enabled;
		})).Build();
	}

	public void UpdateSingleton()
	{
		if (!_enabled)
		{
			return;
		}
		foreach (Vector3Int connectionCoordinate in _ziplineConnectionService.GetConnectionCoordinates())
		{
			_meshDrawer.DrawAtCoordinates(connectionCoordinate, 0f);
		}
	}
}
