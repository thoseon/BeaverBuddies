using System;
using System.Collections.Generic;
using System.IO;
using Timberborn.AssetSystem;
using Timberborn.Timbermesh;
using UnityEngine;

namespace Timberborn.PrefabOptimization;

internal class TimbermeshPrefabOptimizer : IPrefabOptimizer
{
	private readonly IAssetLoader _assetLoader;

	private readonly TimbermeshImporter _timbermeshImporter;

	public TimbermeshPrefabOptimizer(IAssetLoader assetLoader, TimbermeshImporter timbermeshImporter)
	{
		_assetLoader = assetLoader;
		_timbermeshImporter = timbermeshImporter;
	}

	public void Optimize(GameObject prefab)
	{
		TimbermeshDescription[] componentsInChildren = prefab.GetComponentsInChildren<TimbermeshDescription>(includeInactive: true);
		ImportTimbermeshModels(componentsInChildren);
	}

	private void ImportTimbermeshModels(IReadOnlyList<TimbermeshDescription> timbermeshDescriptions)
	{
		for (int i = 0; i < timbermeshDescriptions.Count; i++)
		{
			ImportTimbermeshModel(timbermeshDescriptions[i]);
		}
	}

	private void ImportTimbermeshModel(TimbermeshDescription timbermeshDescription)
	{
		try
		{
			using MemoryStream stream = new MemoryStream(_assetLoader.Load<BinaryData>(timbermeshDescription.ModelName).Bytes);
			_timbermeshImporter.Import((Stream)stream, timbermeshDescription.transform);
		}
		catch (Exception)
		{
			Debug.LogError("Failed to import timbermesh model " + timbermeshDescription.ModelName + ".");
			throw;
		}
	}
}
