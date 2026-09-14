using System;
using System.Collections.Generic;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Rendering;

public class ColoredMaterialCache : IUnloadableSingleton
{
	private readonly struct MaterialKey(Material initialMaterial, MaterialProperties materialProperties) : IEquatable<MaterialKey>
	{
		private readonly Material _initialMaterial = initialMaterial;

		private readonly MaterialProperties _materialProperties = materialProperties;

		public bool Equals(MaterialKey other)
		{
			if (_initialMaterial.Equals(other._initialMaterial))
			{
				return _materialProperties.Equals(other._materialProperties);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is MaterialKey other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (_initialMaterial.GetHashCode() * 397) ^ _materialProperties.GetHashCode();
		}
	}

	private readonly Dictionary<MaterialKey, Material> _cachedMaterials = new Dictionary<MaterialKey, Material>();

	private readonly Dictionary<Material, Material> _coloredToInitial = new Dictionary<Material, Material>();

	public Material GetCachedMaterial(Material inputMaterial, MaterialProperties materialProperties, out bool isNew)
	{
		Material valueOrDefault = _coloredToInitial.GetValueOrDefault(inputMaterial, inputMaterial);
		MaterialKey materialKey = new MaterialKey(valueOrDefault, materialProperties);
		if (_cachedMaterials.TryGetValue(materialKey, out var value))
		{
			isNew = false;
			return value;
		}
		Material result = CreateMaterial(inputMaterial, valueOrDefault, materialKey);
		isNew = true;
		return result;
	}

	public void Unload()
	{
		foreach (var (material3, _) in _coloredToInitial)
		{
			if (material3 != null)
			{
				UnityEngine.Object.Destroy(material3);
			}
		}
	}

	private Material CreateMaterial(Material inputMaterial, Material initialMaterial, MaterialKey materialKey)
	{
		Material material = new Material(inputMaterial);
		_cachedMaterials.Add(materialKey, material);
		_coloredToInitial.Add(material, initialMaterial);
		return material;
	}
}
