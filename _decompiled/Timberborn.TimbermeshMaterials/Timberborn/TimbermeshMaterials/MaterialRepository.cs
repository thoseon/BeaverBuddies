using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.AssetSystem;
using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;
using Timberborn.Timbermesh;
using UnityEngine;

namespace Timberborn.TimbermeshMaterials;

internal class MaterialRepository : ILoadableSingleton, IMaterialRepository
{
	private static readonly string DefaultMaterialPath = "Materials/Common/Empty";

	private readonly ISpecService _specService;

	private readonly IAssetLoader _assetLoader;

	private readonly ImmutableArray<IMaterialCollectionIdsProvider> _materialCollectionProviders;

	private readonly Dictionary<string, Material> _materials = new Dictionary<string, Material>();

	private Material _defaultMaterial;

	public MaterialRepository(ISpecService specService, IAssetLoader assetLoader, IEnumerable<IMaterialCollectionIdsProvider> materialCollectionProviders)
	{
		_specService = specService;
		_assetLoader = assetLoader;
		_materialCollectionProviders = materialCollectionProviders.ToImmutableArray();
	}

	public void Load()
	{
		_defaultMaterial = _assetLoader.Load<Material>(DefaultMaterialPath);
		foreach (Material item in GetMaterials().Distinct())
		{
			if (!_materials.TryAdd(item.name, item))
			{
				throw new InvalidOperationException("Material " + item.name + " is already loaded.");
			}
		}
	}

	public Material GetMaterial(string materialName)
	{
		if (string.IsNullOrWhiteSpace(materialName))
		{
			return _defaultMaterial;
		}
		if (_materials.TryGetValue(materialName, out var value))
		{
			return value;
		}
		throw new ArgumentException("Material " + materialName + " not found in repository.");
	}

	private IEnumerable<Material> GetMaterials()
	{
		ImmutableArray<MaterialCollectionSpec> materialCollectionSpecs = _specService.GetSpecs<MaterialCollectionSpec>().ToImmutableArray();
		foreach (IMaterialCollectionIdsProvider materialCollectionProvider in _materialCollectionProviders)
		{
			IEnumerable<string> materialCollectionIds = materialCollectionProvider.GetMaterialCollectionIds();
			foreach (string materialCollectionId in materialCollectionIds)
			{
				foreach (MaterialCollectionSpec item in materialCollectionSpecs.Where((MaterialCollectionSpec s) => s.CollectionId == materialCollectionId))
				{
					foreach (AssetRef<Material> material in item.Materials)
					{
						yield return material.Asset;
					}
				}
			}
		}
	}
}
