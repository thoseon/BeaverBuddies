using System.Collections.Generic;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.PrefabOptimization;

internal class VertexColorPrefabOptimizer : IPrefabOptimizer
{
	private static readonly int MaxExpectedRegistrySize = 50;

	private readonly MaterialPropertyProvider _materialPropertyProvider;

	private readonly Dictionary<IMaterialProperties, Material> _materialRegistry = new Dictionary<IMaterialProperties, Material>();

	public VertexColorPrefabOptimizer(MaterialPropertyProvider materialPropertyProvider)
	{
		_materialPropertyProvider = materialPropertyProvider;
	}

	public void Optimize(GameObject prefab)
	{
		MeshRenderer[] componentsInChildren = prefab.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			if (meshRenderer.sharedMaterials.Length == 0)
			{
				continue;
			}
			MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
			Mesh sharedMesh = component.sharedMesh;
			Mesh mesh = Object.Instantiate(sharedMesh);
			mesh.name = sharedMesh.name;
			int vertexCount = mesh.vertexCount;
			Color32[] colors = sharedMesh.colors32;
			Color32[] array = new Color32[vertexCount];
			for (int j = 0; j < vertexCount; j++)
			{
				array[j] = Color.white;
			}
			int subMeshCount = sharedMesh.subMeshCount;
			Material[] array2 = new Material[subMeshCount];
			for (int k = 0; k < subMeshCount; k++)
			{
				Material material = meshRenderer.sharedMaterials[k];
				if (material != null)
				{
					IMaterialProperties properties = _materialPropertyProvider.GetProperties(material);
					if (properties != null)
					{
						IMaterialProperties withoutColor = properties.GetWithoutColor();
						array2[k] = GetMaterialFromProperties(withoutColor, material);
						int[] indices = sharedMesh.GetIndices(k);
						Color32 color = properties.Color;
						foreach (int num in indices)
						{
							Color32 color2 = (colors.IsEmpty() ? ((Color32)Color.white) : colors[num]);
							array[num] = new Color32((byte)((float)(color2.r * color.r) / 256f), (byte)((float)(color2.g * color.g) / 256f), (byte)((float)(color2.b * color.b) / 256f), (byte)((float)(color2.a * color.a) / 256f));
						}
						continue;
					}
				}
				array2[k] = material;
			}
			meshRenderer.sharedMaterials = array2;
			mesh.colors32 = array;
			component.sharedMesh = mesh;
		}
	}

	private Material GetMaterialFromProperties(IMaterialProperties properties, Material originalMaterial)
	{
		return _materialRegistry.GetOrAdd(properties, delegate
		{
			int num = _materialRegistry.Count + 1;
			Material material = new Material(Shader.Find(originalMaterial.shader.name))
			{
				name = $"{originalMaterial.name} - Uncolored{num}"
			};
			properties.ApplyToMaterial(material);
			if (num > MaxExpectedRegistrySize && Application.isEditor)
			{
				Debug.LogWarning("The VertexColorPrefabOptimizer registry size" + $" is now {num}" + $", exceeding the expected size of {MaxExpectedRegistrySize}");
			}
			return material;
		});
	}
}
