using UnityEngine;
using UnityEngine.Rendering;

namespace Timberborn.BlockObjectModelSystem;

public static class GameObjectExtensions
{
	public static void ToggleModelVisibility(this GameObject model, bool showModel, bool showShadows)
	{
		if ((bool)model)
		{
			ToggleRenderers(model, showModel, showShadows);
			ToggleColliders(model, showModel);
		}
	}

	private static void ToggleRenderers(GameObject model, bool showModel, bool showShadows)
	{
		Renderer[] componentsInChildren = model.GetComponentsInChildren<Renderer>(includeInactive: true);
		foreach (Renderer renderer in componentsInChildren)
		{
			if (showModel | showShadows)
			{
				renderer.enabled = true;
				renderer.shadowCastingMode = (showShadows ? (showModel ? ShadowCastingMode.On : ShadowCastingMode.ShadowsOnly) : ShadowCastingMode.Off);
			}
			else
			{
				renderer.enabled = false;
			}
		}
	}

	private static void ToggleColliders(GameObject model, bool showModel)
	{
		Collider[] componentsInChildren = model.GetComponentsInChildren<Collider>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = showModel;
		}
	}
}
