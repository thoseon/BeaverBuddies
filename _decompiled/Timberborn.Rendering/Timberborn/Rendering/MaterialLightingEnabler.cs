using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Rendering;

internal class MaterialLightingEnabler : ILoadableSingleton
{
	private static readonly int LightingStrengthMultiplierId = Shader.PropertyToID("LightingStrengthMultiplier");

	private static readonly float MaxStrength = 4f;

	private static readonly float LightingStrengthMultiplier = 1f / MaxStrength * 255f;

	public void Load()
	{
		Shader.SetGlobalFloat(LightingStrengthMultiplierId, 1f / LightingStrengthMultiplier);
	}

	public void EnableLighting(GameObject target, float? strength = 1f)
	{
		SetStrengthInRenderers(target.GetComponentsInChildren<MeshRenderer>(includeInactive: true), strength);
	}

	public void EnableLighting(BaseComponent entity, float? strength = 1f)
	{
		SetStrengthInRenderers(entity.GetComponent<MaterialLightingRenderers>().Renderers, strength);
	}

	public void DisableLighting(BaseComponent entity)
	{
		EnableLighting(entity, 0f);
	}

	private static void SetStrengthInRenderers(IReadOnlyList<MeshRenderer> renderers, float? strength)
	{
		uint shaderUserValue = (uint)((strength ?? 1f) * LightingStrengthMultiplier);
		for (int i = 0; i < renderers.Count; i++)
		{
			renderers[i].SetShaderUserValue(shaderUserValue);
		}
	}
}
