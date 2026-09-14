using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using UnityEngine;

namespace Timberborn.Rendering;

public class MaterialColorer
{
	private static readonly int EmissionColorProperty = Shader.PropertyToID("_EmissionColor");

	private static readonly int GrayscaleProperty = Shader.PropertyToID("_Grayscale");

	private static readonly int LightingColorProperty = Shader.PropertyToID("_LightingColor");

	private static readonly Color UnhighlightedColor = new Color(0f, 0f, 0f, 0f);

	private static readonly float Grayscale = 1f;

	private static readonly float Colored = 0f;

	private static readonly Color DefaultLightingColor = Color.white;

	private readonly List<MeshRenderer> _rendererCache = new List<MeshRenderer>();

	private readonly List<Material> _readMaterialCache = new List<Material>();

	private readonly List<Material> _writeMaterialCache = new List<Material>();

	private readonly ColoredMaterialCache _coloredMaterialCache;

	private readonly MaterialLightingEnabler _materialLightingEnabler;

	internal MaterialColorer(ColoredMaterialCache coloredMaterialCache, MaterialLightingEnabler materialLightingEnabler)
	{
		_coloredMaterialCache = coloredMaterialCache;
		_materialLightingEnabler = materialLightingEnabler;
	}

	public void EnableGrayscale(GameObject root)
	{
		SetProperties(root, null, Grayscale, null);
	}

	public void DisableGrayscale(GameObject root)
	{
		SetProperties(root, null, Colored, null);
	}

	public void SetLightingColor(GameObject root, Color lightingColor)
	{
		SetProperties(root, null, null, lightingColor);
	}

	public void SetEmissionColor(BaseComponent entity, Color emissionColor)
	{
		SetProperties(entity, emissionColor, null, null);
	}

	public void ResetEmissionColor(BaseComponent entity)
	{
		SetProperties(entity, UnhighlightedColor, null, null);
	}

	public void EnableGrayscale(BaseComponent entity)
	{
		SetProperties(entity, null, Grayscale, null);
	}

	public void SetLightingColor(BaseComponent target, Color lightingColor)
	{
		SetProperties(target, null, null, lightingColor);
	}

	public void EnableLighting(BaseComponent entity, float? strength = null)
	{
		_materialLightingEnabler.EnableLighting(entity, strength);
	}

	public void DisableLighting(BaseComponent entity)
	{
		_materialLightingEnabler.DisableLighting(entity);
	}

	public void EnableLightingAndDisableChanges(BaseComponent entity, GameObject root)
	{
		_materialLightingEnabler.EnableLighting(root, 1f);
		MaterialLightingRenderers component = entity.GetComponent<MaterialLightingRenderers>();
		MeshRenderer[] componentsInChildren = root.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		foreach (MeshRenderer renderer in componentsInChildren)
		{
			component.DisableRendering(renderer);
		}
	}

	private void SetProperties(BaseComponent entity, Color? color, float? grayscale, Color? lightingColor)
	{
		EntityMaterials component = entity.GetComponent<EntityMaterials>();
		if (component != null)
		{
			SetEntityMaterialProperties(component, entity.GameObject, color, grayscale, lightingColor);
		}
		else
		{
			SetCachedMaterialProperties(entity.GameObject, color, grayscale, lightingColor);
		}
	}

	private void SetProperties(GameObject root, Color? color, float? grayscale, Color? lightingColor)
	{
		EntityMaterials componentInParentSlow = root.GetComponentInParentSlow<EntityMaterials>();
		if (componentInParentSlow != null)
		{
			SetEntityMaterialProperties(componentInParentSlow, root, color, grayscale, lightingColor);
		}
		else
		{
			SetCachedMaterialProperties(root, color, grayscale, lightingColor);
		}
	}

	private void SetEntityMaterialProperties(EntityMaterials entityMaterials, GameObject root, Color? color, float? grayscale, Color? lightingColor)
	{
		entityMaterials.GetChildMaterials(root.transform, _readMaterialCache);
		foreach (Material item in _readMaterialCache)
		{
			MaterialProperties materialProperties = GetMaterialProperties(item, color, grayscale, lightingColor);
			ColorMaterial(item, materialProperties);
		}
		_readMaterialCache.Clear();
	}

	private void SetCachedMaterialProperties(GameObject root, Color? color, float? grayscale, Color? lightingColor)
	{
		root.GetComponentsInChildren(includeInactive: true, _rendererCache);
		foreach (MeshRenderer item in _rendererCache)
		{
			_writeMaterialCache.Clear();
			item.GetSharedMaterials(_readMaterialCache);
			for (int i = 0; i < _readMaterialCache.Count; i++)
			{
				Material material = _readMaterialCache[i];
				MaterialProperties materialProperties = GetMaterialProperties(material, color, grayscale, lightingColor);
				Material cachedMaterial = GetCachedMaterial(material, materialProperties);
				_writeMaterialCache.Add(cachedMaterial);
			}
			item.SetSharedMaterials(_writeMaterialCache);
			_readMaterialCache.Clear();
		}
		_rendererCache.Clear();
	}

	private static MaterialProperties GetMaterialProperties(Material material, Color? color, float? grayscale, Color? lightingColor)
	{
		Color color2 = color ?? GetEmissionColor(material, UnhighlightedColor);
		float grayscale2 = grayscale ?? GetGrayscale(material, Colored);
		Color lightingColor2 = lightingColor ?? GetLightingColor(material, DefaultLightingColor);
		return new MaterialProperties(color2, grayscale2, lightingColor2);
	}

	private Material GetCachedMaterial(Material material, MaterialProperties materialProperties)
	{
		Material cachedMaterial = _coloredMaterialCache.GetCachedMaterial(material, materialProperties, out var isNew);
		if (isNew)
		{
			ColorMaterial(cachedMaterial, materialProperties);
		}
		return cachedMaterial;
	}

	private static void ColorMaterial(Material material, MaterialProperties materialProperties)
	{
		material.SetColor(EmissionColorProperty, materialProperties.Color);
		material.SetFloat(GrayscaleProperty, materialProperties.Grayscale);
		material.SetColor(LightingColorProperty, materialProperties.LightingColor);
	}

	private static Color GetEmissionColor(Material material, Color defaultColor)
	{
		if (!MaterialSupportsEmission(material))
		{
			return defaultColor;
		}
		return material.GetColor(EmissionColorProperty);
	}

	private static float GetGrayscale(Material material, float defaultGrayscale)
	{
		if (!material.HasProperty(GrayscaleProperty))
		{
			return defaultGrayscale;
		}
		return material.GetFloat(GrayscaleProperty);
	}

	private static Color GetLightingColor(Material material, Color defaultLightingColor)
	{
		if (!material.HasProperty(LightingColorProperty))
		{
			return defaultLightingColor;
		}
		return material.GetColor(LightingColorProperty);
	}

	private static bool MaterialSupportsEmission(Material material)
	{
		return material.HasProperty(EmissionColorProperty);
	}
}
