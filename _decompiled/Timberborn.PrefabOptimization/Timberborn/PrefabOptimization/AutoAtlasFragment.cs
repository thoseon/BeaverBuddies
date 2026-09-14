using UnityEngine;

namespace Timberborn.PrefabOptimization;

public class AutoAtlasFragment
{
	public string AtlasName { get; }

	public Texture2D CombinedMainTex { get; }

	public Texture2D CombinedBumpMap { get; }

	public Texture2D CombinedColorMask { get; }

	public Texture2D CombinedAmbientOcclusion { get; }

	public Texture2D CombinedMetallicGlossMap { get; }

	public Texture2D CombinedLightingMap { get; }

	public Vector2 UVScale { get; }

	public Vector2 UVOffset { get; }

	public AutoAtlasFragment(string atlasName, Texture2D combinedMainTex, Texture2D combinedBumpMap, Texture2D combinedColorMask, Texture2D combinedAmbientOcclusion, Texture2D combinedMetallicGlossMap, Texture2D combinedLightingMap, Vector2 uvScale, Vector2 uvOffset)
	{
		AtlasName = atlasName;
		CombinedMainTex = combinedMainTex;
		CombinedBumpMap = combinedBumpMap;
		CombinedColorMask = combinedColorMask;
		CombinedAmbientOcclusion = combinedAmbientOcclusion;
		CombinedMetallicGlossMap = combinedMetallicGlossMap;
		CombinedLightingMap = combinedLightingMap;
		UVScale = uvScale;
		UVOffset = uvOffset;
	}

	public void DestroyTextures()
	{
		Object.Destroy(CombinedMainTex);
		Object.Destroy(CombinedBumpMap);
		Object.Destroy(CombinedColorMask);
		Object.Destroy(CombinedAmbientOcclusion);
		Object.Destroy(CombinedMetallicGlossMap);
		Object.Destroy(CombinedLightingMap);
	}
}
