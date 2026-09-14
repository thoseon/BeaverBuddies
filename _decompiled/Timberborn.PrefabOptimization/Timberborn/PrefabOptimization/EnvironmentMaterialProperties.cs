using UnityEngine;

namespace Timberborn.PrefabOptimization;

internal record EnvironmentMaterialProperties : IMaterialProperties
{
	public Color32 Color { get; private init; }

	public Texture2D MainTex { get; init; }

	public Texture2D BumpMap { get; init; }

	public Texture2D ColorMask { get; init; }

	public Texture2D AmbientOcclusion { get; init; }

	public Texture2D MetallicGlossMap { get; init; }

	public Texture2D LightingMap { get; init; }

	private bool CutoutWithAlpha { get; init; }

	private float Cutoff { get; init; }

	private Texture2D CutoutTex { get; init; }

	private Texture2D DetailAlbedoMap { get; init; }

	private Texture2D DetailAlbedoMap2 { get; init; }

	private Texture2D DetailAlbedoMap3 { get; init; }

	private Color32 DetailAlbedoColor2 { get; init; }

	private float DetailAlbedoGradient3 { get; init; }

	private Color32 EmissionColor { get; init; }

	private bool MainUVFromCoordinates { get; init; }

	private float MainUVFromCoordinatesScale { get; init; }

	private bool EnableInstancing { get; init; }

	private static readonly int ColorProperty = Shader.PropertyToID("_Color");

	private static readonly int MainTexProperty = Shader.PropertyToID("_MainTex");

	private static readonly int ColorMaskProperty = Shader.PropertyToID("_ColorMask");

	private static readonly int AmbientOcclusionProperty = Shader.PropertyToID("_AmbientOcclusion");

	private static readonly int MetallicGlossMapProperty = Shader.PropertyToID("_MetallicGlossMap");

	private static readonly int LightingMapProperty = Shader.PropertyToID("_LightingMap");

	private static readonly int CutoutWithAlphaProperty = Shader.PropertyToID("_CutoutWithAlpha");

	private static readonly int CutoutTexProperty = Shader.PropertyToID("_CutoutTex");

	private static readonly int CutoffProperty = Shader.PropertyToID("_Cutoff");

	private static readonly int BumpMapProperty = Shader.PropertyToID("_BumpMap");

	private static readonly int DetailAlbedoMapProperty = Shader.PropertyToID("_DetailAlbedoMap");

	private static readonly int DetailAlbedoMap2Property = Shader.PropertyToID("_DetailAlbedoMap2");

	private static readonly int DetailAlbedoMap3Property = Shader.PropertyToID("_DetailAlbedoMap3");

	private static readonly int DetailAlbedoColor2Property = Shader.PropertyToID("_DetailAlbedoUV2Color");

	private static readonly int DetailAlbedoGradient3Property = Shader.PropertyToID("_DetailAlbedoUV3Gradient");

	private static readonly int EmissionColorProperty = Shader.PropertyToID("_EmissionColor");

	private static readonly int MainUVFromCoordinatesProperty = Shader.PropertyToID("_MainUVFromCoordinates");

	private static readonly int MainUVFromCoordinatesScaleProperty = Shader.PropertyToID("_MainUVFromCoordinatesScale");

	public static EnvironmentMaterialProperties FromMaterial(Material material)
	{
		return new EnvironmentMaterialProperties
		{
			Color = material.GetColor(ColorProperty),
			MainTex = (Texture2D)material.GetTexture(MainTexProperty),
			ColorMask = (Texture2D)material.GetTexture(ColorMaskProperty),
			AmbientOcclusion = (Texture2D)material.GetTexture(AmbientOcclusionProperty),
			MetallicGlossMap = (Texture2D)material.GetTexture(MetallicGlossMapProperty),
			LightingMap = (Texture2D)material.GetTexture(LightingMapProperty),
			CutoutWithAlpha = (material.GetFloat(CutoutWithAlphaProperty) > 0.5f),
			CutoutTex = (Texture2D)material.GetTexture(CutoutTexProperty),
			Cutoff = material.GetFloat(CutoffProperty),
			BumpMap = (Texture2D)material.GetTexture(BumpMapProperty),
			DetailAlbedoMap = (Texture2D)material.GetTexture(DetailAlbedoMapProperty),
			DetailAlbedoMap2 = (Texture2D)material.GetTexture(DetailAlbedoMap2Property),
			DetailAlbedoMap3 = (Texture2D)material.GetTexture(DetailAlbedoMap3Property),
			DetailAlbedoColor2 = material.GetColor(DetailAlbedoColor2Property),
			DetailAlbedoGradient3 = material.GetFloat(DetailAlbedoGradient3Property),
			EmissionColor = material.GetColor(EmissionColorProperty),
			MainUVFromCoordinates = (material.GetFloat(MainUVFromCoordinatesProperty) > 0.5f),
			MainUVFromCoordinatesScale = material.GetFloat(MainUVFromCoordinatesScaleProperty),
			EnableInstancing = material.enableInstancing
		};
	}

	public void ApplyToMaterial(Material material)
	{
		material.SetColor(ColorProperty, Color);
		material.SetTexture(MainTexProperty, MainTex);
		material.SetTexture(BumpMapProperty, BumpMap);
		material.SetTexture(ColorMaskProperty, ColorMask);
		material.SetTexture(AmbientOcclusionProperty, AmbientOcclusion);
		material.SetTexture(MetallicGlossMapProperty, MetallicGlossMap);
		material.SetTexture(LightingMapProperty, LightingMap);
		material.SetFloat(CutoutWithAlphaProperty, CutoutWithAlpha ? 1 : 0);
		material.SetTexture(CutoutTexProperty, CutoutTex);
		material.SetFloat(CutoffProperty, Cutoff);
		material.SetTexture(DetailAlbedoMapProperty, DetailAlbedoMap);
		material.SetTexture(DetailAlbedoMap2Property, DetailAlbedoMap2);
		material.SetTexture(DetailAlbedoMap3Property, DetailAlbedoMap3);
		material.SetColor(DetailAlbedoColor2Property, DetailAlbedoColor2);
		material.SetFloat(DetailAlbedoGradient3Property, DetailAlbedoGradient3);
		material.SetColor(EmissionColorProperty, EmissionColor);
		material.SetFloat(MainUVFromCoordinatesProperty, MainUVFromCoordinates ? 1 : 0);
		material.SetFloat(MainUVFromCoordinatesScaleProperty, MainUVFromCoordinatesScale);
		material.enableInstancing = EnableInstancing;
	}

	public IMaterialProperties GetWithoutColor()
	{
		return this with
		{
			Color = UnityEngine.Color.white
		};
	}
}
