using UnityEngine;

namespace Timberborn.PrefabOptimization;

internal class MaterialPropertyProvider
{
	public IMaterialProperties GetProperties(Material material)
	{
		string name = material.shader.name;
		if (!(name == "Shader Graphs/EnvironmentURP"))
		{
			if (name == "Shader Graphs/VegetationURP")
			{
				return VegetationMaterialProperties.FromMaterial(material);
			}
			return null;
		}
		return EnvironmentMaterialProperties.FromMaterial(material);
	}
}
