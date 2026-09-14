using System.Collections.Generic;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.PrefabOptimization;

public class AutoAtlasingPrefabOptimizer : IPrefabOptimizer
{
	private static readonly string EnvironmentShaderName = "Shader Graphs/EnvironmentURP";

	private static readonly int MaxExpectedRegistrySize = 20;

	private readonly AutoAtlaser _autoAtlaser;

	private readonly Dictionary<EnvironmentMaterialProperties, Material> _materialRegistry = new Dictionary<EnvironmentMaterialProperties, Material>();

	public AutoAtlasingPrefabOptimizer(AutoAtlaser autoAtlaser)
	{
		_autoAtlaser = autoAtlaser;
	}

	public void Optimize(GameObject prefab)
	{
		MeshRenderer[] componentsInChildren = prefab.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			OptimizeMeshRenderer(meshRenderer, prefab.name);
		}
	}

	private void OptimizeMeshRenderer(MeshRenderer meshRenderer, string usageName)
	{
		MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
		Mesh sharedMesh = component.sharedMesh;
		Material[] sharedMaterials = meshRenderer.sharedMaterials;
		Vector2[] uv = sharedMesh.uv;
		if (sharedMaterials.IsEmpty() || uv.IsEmpty())
		{
			return;
		}
		int subMeshCount = sharedMesh.subMeshCount;
		Material[] array = (Material[])sharedMaterials.Clone();
		Vector2[] array2 = (Vector2[])uv.Clone();
		for (int i = 0; i < subMeshCount; i++)
		{
			Material material = sharedMaterials[i];
			if (!(material != null) || !(material.shader.name == EnvironmentShaderName))
			{
				continue;
			}
			EnvironmentMaterialProperties environmentMaterialProperties = EnvironmentMaterialProperties.FromMaterial(material);
			Texture2D mainTex = environmentMaterialProperties.MainTex;
			Texture2D bumpMap = environmentMaterialProperties.BumpMap;
			Texture2D colorMask = environmentMaterialProperties.ColorMask;
			Texture2D ambientOcclusion = environmentMaterialProperties.AmbientOcclusion;
			Texture2D metallicGlossMap = environmentMaterialProperties.MetallicGlossMap;
			Texture2D lightingMap = environmentMaterialProperties.LightingMap;
			AutoAtlasKey key = new AutoAtlasKey(mainTex, bumpMap, colorMask, ambientOcclusion, metallicGlossMap, lightingMap);
			if (_autoAtlaser.TryGetAutoAtlasFragment(key, usageName, out var autoAtlasFragment))
			{
				EnvironmentMaterialProperties properties = environmentMaterialProperties with
				{
					MainTex = autoAtlasFragment.CombinedMainTex,
					BumpMap = autoAtlasFragment.CombinedBumpMap,
					ColorMask = autoAtlasFragment.CombinedColorMask,
					AmbientOcclusion = autoAtlasFragment.CombinedAmbientOcclusion,
					MetallicGlossMap = autoAtlasFragment.CombinedMetallicGlossMap,
					LightingMap = autoAtlasFragment.CombinedLightingMap
				};
				array[i] = GetMaterialFromProperties(properties, autoAtlasFragment.AtlasName);
				int[] indices = sharedMesh.GetIndices(i);
				for (int j = 0; j < indices.Length; j++)
				{
					array2[indices[j]] = uv[indices[j]] * autoAtlasFragment.UVScale + autoAtlasFragment.UVOffset;
				}
			}
		}
		Mesh mesh = Object.Instantiate(sharedMesh);
		mesh.name = sharedMesh.name;
		meshRenderer.sharedMaterials = array;
		mesh.uv = array2;
		component.sharedMesh = mesh;
	}

	private Material GetMaterialFromProperties(EnvironmentMaterialProperties properties, string atlasName)
	{
		return _materialRegistry.GetOrAdd(properties, delegate
		{
			int num = _materialRegistry.Count + 1;
			Material material = new Material(Shader.Find(EnvironmentShaderName))
			{
				name = $"{atlasName}{num}"
			};
			properties.ApplyToMaterial(material);
			if (num > MaxExpectedRegistrySize)
			{
				Debug.LogWarning("The AutoAtlasingPrefabOptimizer registry size" + $" is now {num}" + $", exceeding the expected size of {MaxExpectedRegistrySize}");
			}
			return material;
		});
	}
}
