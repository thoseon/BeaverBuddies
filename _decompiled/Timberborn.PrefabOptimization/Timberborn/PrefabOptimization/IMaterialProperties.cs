using UnityEngine;

namespace Timberborn.PrefabOptimization;

internal interface IMaterialProperties
{
	Color32 Color { get; }

	void ApplyToMaterial(Material material);

	IMaterialProperties GetWithoutColor();
}
