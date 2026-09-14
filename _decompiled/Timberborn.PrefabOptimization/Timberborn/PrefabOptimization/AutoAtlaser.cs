using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.SingletonSystem;
using Timberborn.TextureOperations;
using UnityEngine;

namespace Timberborn.PrefabOptimization;

public class AutoAtlaser : ILoadableSingleton, IUnloadableSingleton
{
	private static readonly int MaxUniqueAutoAtlases = 1;

	private readonly TextureFactory _textureFactory;

	private readonly ISpecService _specService;

	private Dictionary<AutoAtlasKey, AutoAtlasSpec> _keyToSpec;

	private HashSet<AutoAtlasSpec> _generatedAutoAtlases;

	private Dictionary<AutoAtlasKey, AutoAtlasFragment> _fragments;

	private Dictionary<AutoAtlasSpec, HashSet<string>> _usages;

	private readonly ReusableColorArray _reusableColorArray = new ReusableColorArray();

	public AutoAtlaser(TextureFactory textureFactory, ISpecService specService)
	{
		_textureFactory = textureFactory;
		_specService = specService;
	}

	public void Load()
	{
		_keyToSpec = new Dictionary<AutoAtlasKey, AutoAtlasSpec>();
		_generatedAutoAtlases = new HashSet<AutoAtlasSpec>();
		_fragments = new Dictionary<AutoAtlasKey, AutoAtlasFragment>();
		_usages = new Dictionary<AutoAtlasSpec, HashSet<string>>();
		PopulateKeyToSpecs();
	}

	public void Unload()
	{
		foreach (AutoAtlasFragment value in _fragments.Values)
		{
			value.DestroyTextures();
		}
		PrintUsagesIfTooManyAtlases();
	}

	public bool TryGetAutoAtlasFragment(AutoAtlasKey key, string usageName, out AutoAtlasFragment autoAtlasFragment)
	{
		if (_keyToSpec.TryGetValue(key, out var value))
		{
			if (_generatedAutoAtlases.Add(value))
			{
				_usages[value] = new HashSet<string>();
				GenerateAutoAtlasFragments(value);
				WarnIfTooManyAtlases();
			}
			autoAtlasFragment = _fragments[key];
			_usages[value].Add(usageName);
			return true;
		}
		autoAtlasFragment = null;
		return false;
	}

	private void PopulateKeyToSpecs()
	{
		foreach (AutoAtlasSpec autoAtlase in _specService.GetSingleSpec<AutoAtlaserSpec>().AutoAtlases)
		{
			foreach (AssetRef<Material> fragment in autoAtlase.Fragments)
			{
				_keyToSpec[KeyFromMaterial(fragment.Asset)] = autoAtlase;
			}
		}
	}

	private void GenerateAutoAtlasFragments(AutoAtlasSpec autoAtlasSpec)
	{
		ImmutableArray<AssetRef<Material>> fragments = autoAtlasSpec.Fragments;
		if (!fragments.IsEmpty())
		{
			int num = CalculateSizeMultiplier(fragments.Length);
			Texture2D combinedMainTex = CombineFragments(autoAtlasSpec, num, (EnvironmentMaterialProperties fragment) => fragment.MainTex, new Color32(0, 0, 0, byte.MaxValue), "MainTex");
			Texture2D combinedBumpMap = CombineFragments(autoAtlasSpec, num, (EnvironmentMaterialProperties fragment) => fragment.BumpMap, new Color32(128, 128, byte.MaxValue, byte.MaxValue), "BumpMap");
			Texture2D combinedColorMask = CombineFragments(autoAtlasSpec, num, (EnvironmentMaterialProperties fragment) => fragment.ColorMask, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), "ColorMask");
			Texture2D combinedAmbientOcclusion = CombineFragments(autoAtlasSpec, num, (EnvironmentMaterialProperties fragment) => fragment.AmbientOcclusion, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), "AmbientOcclusion");
			Texture2D combinedMetallicGlossMap = CombineFragments(autoAtlasSpec, num, (EnvironmentMaterialProperties fragment) => fragment.MetallicGlossMap, new Color32(0, 0, 0, 0), "MetallicGlossMap");
			Texture2D combinedLightingMap = CombineFragments(autoAtlasSpec, num, (EnvironmentMaterialProperties fragment) => fragment.LightingMap, new Color32(0, 0, 0, 0), "LightingMap");
			int num2 = 0;
			int num3 = 0;
			Vector2 uvScale = Vector2.one / num;
			foreach (AssetRef<Material> item in fragments)
			{
				AutoAtlasKey key = KeyFromMaterial(item.Asset);
				AutoAtlasFragment value = new AutoAtlasFragment(uvOffset: new Vector2((float)num2 / (float)num, (float)num3 / (float)num), atlasName: autoAtlasSpec.Name, combinedMainTex: combinedMainTex, combinedBumpMap: combinedBumpMap, combinedColorMask: combinedColorMask, combinedAmbientOcclusion: combinedAmbientOcclusion, combinedMetallicGlossMap: combinedMetallicGlossMap, combinedLightingMap: combinedLightingMap, uvScale: uvScale);
				_fragments.Add(key, value);
				num2++;
				if (num2 == num)
				{
					num2 = 0;
					num3++;
				}
			}
		}
		_reusableColorArray.Clear();
	}

	private static AutoAtlasKey KeyFromMaterial(Material fragmentMaterial)
	{
		EnvironmentMaterialProperties environmentMaterialProperties = EnvironmentMaterialProperties.FromMaterial(fragmentMaterial);
		return new AutoAtlasKey(NormalizeNull(environmentMaterialProperties.MainTex), NormalizeNull(environmentMaterialProperties.BumpMap), NormalizeNull(environmentMaterialProperties.ColorMask), NormalizeNull(environmentMaterialProperties.AmbientOcclusion), NormalizeNull(environmentMaterialProperties.MetallicGlossMap), NormalizeNull(environmentMaterialProperties.LightingMap));
	}

	private Texture2D CombineFragments(AutoAtlasSpec autoAtlas, int sizeMultiplier, Func<EnvironmentMaterialProperties, Texture2D> textureSupplier, Color32 defaultColor, string texturePostfix)
	{
		ImmutableArray<AssetRef<Material>> fragments = autoAtlas.Fragments;
		ImmutableArray<Texture2D> immutableArray = (from fragment in fragments
			select textureSupplier(EnvironmentMaterialProperties.FromMaterial(fragment.Asset)) into texture
			where texture
			select texture).ToImmutableArray();
		ImmutableArray<Vector2Int> immutableArray2 = immutableArray.Select((Texture2D texture) => new Vector2Int(texture.width, texture.height)).ToImmutableArray();
		if (!immutableArray2.AllAreEqual())
		{
			throw new ArgumentException("All '" + texturePostfix + "' fragment textures  of atlas '" + autoAtlas.Name + "' must be the same size.");
		}
		Vector2Int vector2Int = ((immutableArray2.Length > 0) ? immutableArray2[0] : Vector2Int.one);
		Vector2Int vector2Int2 = vector2Int * sizeMultiplier;
		FilterMode filterMode = ((immutableArray.Length > 0) ? immutableArray[0].filterMode : FilterMode.Point);
		int anisoLevel = ((immutableArray.Length > 0) ? immutableArray[0].anisoLevel : 0);
		TextureSettings textureSettings = new TextureSettings.Builder().SetSize(vector2Int2.x, vector2Int2.y).SetFilterMode(filterMode).SetAnisoLevel(anisoLevel)
			.SetName(autoAtlas.Name + "-" + texturePostfix)
			.Build();
		Texture2D texture2D = _textureFactory.CreateTexture(textureSettings);
		int num = sizeMultiplier * sizeMultiplier;
		int length = fragments.Length;
		for (int num2 = 0; num2 < num; num2++)
		{
			Texture2D texture2D2 = ((num2 < length) ? textureSupplier(EnvironmentMaterialProperties.FromMaterial(fragments[num2].Asset)) : null);
			Color32[] colors = (texture2D2 ? texture2D2.GetPixels32() : _reusableColorArray.Get(vector2Int.x * vector2Int.y, defaultColor));
			int num3 = num2 % sizeMultiplier;
			int num4 = num2 / sizeMultiplier;
			texture2D.SetPixels32(num3 * vector2Int.x, num4 * vector2Int.y, vector2Int.x, vector2Int.y, colors);
		}
		texture2D.Apply(updateMipmaps: true, makeNoLongerReadable: true);
		return texture2D;
	}

	private void WarnIfTooManyAtlases()
	{
		if (TooManyAtlases())
		{
			Debug.LogWarning($"Too many atlases loaded ({_generatedAutoAtlases.Count})! This should not happen." + " Exit game to print usages.");
		}
	}

	private void PrintUsagesIfTooManyAtlases()
	{
		if (!TooManyAtlases())
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Too many auto atlases loaded! Usages:");
		foreach (KeyValuePair<AutoAtlasSpec, HashSet<string>> usage in _usages)
		{
			stringBuilder.AppendLine(usage.Key.Name);
			foreach (string item in usage.Value)
			{
				stringBuilder.Append("- ");
				stringBuilder.Append(item);
				stringBuilder.AppendLine();
			}
		}
		Debug.LogWarning(stringBuilder.ToString());
	}

	private bool TooManyAtlases()
	{
		return _generatedAutoAtlases.Count((AutoAtlasSpec atlas) => atlas.IsUnique) > MaxUniqueAutoAtlases;
	}

	private static Texture2D NormalizeNull(Texture2D texture)
	{
		if (!texture)
		{
			return null;
		}
		return texture;
	}

	private static int CalculateSizeMultiplier(int numberOfTextures)
	{
		int i;
		for (i = 1; i * i < numberOfTextures; i++)
		{
		}
		return i;
	}
}
