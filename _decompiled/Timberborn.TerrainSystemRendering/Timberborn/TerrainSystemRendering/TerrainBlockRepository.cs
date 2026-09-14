using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Timberborn.AssetSystem;
using Timberborn.PrefabOptimization;
using Timberborn.SingletonSystem;
using Timberborn.Timbermesh;
using UnityEngine;

namespace Timberborn.TerrainSystemRendering;

public class TerrainBlockRepository : ILoadableSingleton
{
	private readonly SurfaceBlockCollectionFactory _surfaceBlockCollectionFactory;

	private readonly IAssetLoader _assetLoader;

	private readonly TimbermeshImporter _timbermeshImporter;

	private SurfaceBlockCollection _surfaceBlockCollection;

	public TerrainBlockRepository(SurfaceBlockCollectionFactory surfaceBlockCollectionFactory, IAssetLoader assetLoader, TimbermeshImporter timbermeshImporter)
	{
		_surfaceBlockCollectionFactory = surfaceBlockCollectionFactory;
		_assetLoader = assetLoader;
		_timbermeshImporter = timbermeshImporter;
	}

	public void Load()
	{
		IEnumerable<LoadedAsset<BinaryData>> binaryData = _assetLoader.LoadAll<BinaryData>("Environment/Land");
		Transform transform = new GameObject("TerrainBlockRepository").transform;
		ImportModels(binaryData, transform);
		Object.Destroy(transform.gameObject);
	}

	public ImmutableArray<IntermediateMesh> GetTerrainBlocks(SurfaceBlockShape shape)
	{
		return _surfaceBlockCollection.GetVariations(shape);
	}

	private void ImportModels(IEnumerable<LoadedAsset<BinaryData>> binaryData, Transform parent)
	{
		List<GameObject> list = new List<GameObject>();
		foreach (LoadedAsset<BinaryData> binaryDatum in binaryData)
		{
			using MemoryStream stream = new MemoryStream(binaryDatum.Asset.Bytes);
			_timbermeshImporter.Import((Stream)stream, parent);
			GameObject gameObject = parent.GetChild(parent.childCount - 1).gameObject;
			gameObject.name = binaryDatum.Asset.name;
			list.Add(gameObject);
		}
		_surfaceBlockCollection = _surfaceBlockCollectionFactory.CreateFromModels(list);
	}
}
