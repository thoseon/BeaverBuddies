using System.Collections.Generic;
using Timberborn.AssetSystem;
using Timberborn.SingletonSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace Timberborn.Localization;

public class PanelTextSettingsUpdater : ILoadableSingleton, IUnloadableSingleton
{
	private static readonly string PanelTextSettingsPath = "UI/Fonts/PanelTextSettings";

	private static readonly string DynamicKeyword = " - Dynamic";

	private static readonly string FallbackKeyword = " - Fallback";

	private static readonly string RegularName = "NotoSans-Regular SDF";

	private static readonly string SymbolsName = "NotoSansSymbols2-Regular";

	private static readonly string JapaneseName = "NotoSansJP-Regular SDF";

	private static readonly string KoreanName = "NotoSansKR-Regular SDF";

	private static readonly string SimplifiedChineseName = "NotoSansSC-Regular SDF";

	private static readonly string TraditionalChineseName = "NotoSansTC-Regular SDF";

	private static readonly string ThaiName = "NotoSansTH-Medium SDF";

	private readonly IAssetLoader _assetLoader;

	private readonly List<FontAsset> _originalFallbackAssets = new List<FontAsset>();

	public PanelTextSettingsUpdater(IAssetLoader assetLoader)
	{
		_assetLoader = assetLoader;
	}

	public void Load()
	{
		List<FontAsset> fallbackFonts = GetFallbackFonts();
		_originalFallbackAssets.AddRange(fallbackFonts);
	}

	public void Update(string languageCode)
	{
		List<FontAsset> fallbackFonts = GetFallbackFonts();
		fallbackFonts.Clear();
		AddDefaultDynamicFonts(fallbackFonts);
		if (languageCode == LocalizationCodes.Japanese)
		{
			AddFontsInJapaneseOrder(fallbackFonts);
		}
		else if (languageCode == LocalizationCodes.Korean)
		{
			AddFontsInKoreanOrder(fallbackFonts);
		}
		else if (languageCode == LocalizationCodes.SimplifiedChinese)
		{
			AddFontsInSimplifiedChineseOrder(fallbackFonts);
		}
		else if (languageCode == LocalizationCodes.TraditionalChinese)
		{
			AddFontsInTraditionalChineseOrder(fallbackFonts);
		}
		else if (languageCode == LocalizationCodes.Thai)
		{
			AddFontsInThaiOrder(fallbackFonts);
		}
		else
		{
			AddFontsInDefaultOrder(fallbackFonts);
		}
	}

	public void Unload()
	{
		List<FontAsset> fallbackFonts = GetFallbackFonts();
		fallbackFonts.Clear();
		fallbackFonts.AddRange(_originalFallbackAssets);
	}

	private List<FontAsset> GetFallbackFonts()
	{
		return _assetLoader.Load<PanelTextSettings>(PanelTextSettingsPath).fallbackFontAssets;
	}

	private void AddDefaultDynamicFonts(List<FontAsset> fallbackFontAssets)
	{
		Add(fallbackFontAssets, RegularName, DynamicKeyword);
		Add(fallbackFontAssets, SymbolsName, DynamicKeyword);
	}

	private void AddFontsInJapaneseOrder(List<FontAsset> fallbackFontAssets)
	{
		Add(fallbackFontAssets, JapaneseName, DynamicKeyword);
		Add(fallbackFontAssets, KoreanName, FallbackKeyword);
		Add(fallbackFontAssets, SimplifiedChineseName, FallbackKeyword);
		Add(fallbackFontAssets, TraditionalChineseName, FallbackKeyword);
		Add(fallbackFontAssets, ThaiName, FallbackKeyword);
	}

	private void AddFontsInKoreanOrder(List<FontAsset> fallbackFontAssets)
	{
		Add(fallbackFontAssets, KoreanName, DynamicKeyword);
		Add(fallbackFontAssets, JapaneseName, FallbackKeyword);
		Add(fallbackFontAssets, SimplifiedChineseName, FallbackKeyword);
		Add(fallbackFontAssets, TraditionalChineseName, FallbackKeyword);
		Add(fallbackFontAssets, ThaiName, FallbackKeyword);
	}

	private void AddFontsInSimplifiedChineseOrder(List<FontAsset> fallbackFontAssets)
	{
		Add(fallbackFontAssets, SimplifiedChineseName, DynamicKeyword);
		Add(fallbackFontAssets, TraditionalChineseName, FallbackKeyword);
		Add(fallbackFontAssets, JapaneseName, FallbackKeyword);
		Add(fallbackFontAssets, KoreanName, FallbackKeyword);
		Add(fallbackFontAssets, ThaiName, FallbackKeyword);
	}

	private void AddFontsInTraditionalChineseOrder(List<FontAsset> fallbackFontAssets)
	{
		Add(fallbackFontAssets, TraditionalChineseName, DynamicKeyword);
		Add(fallbackFontAssets, SimplifiedChineseName, FallbackKeyword);
		Add(fallbackFontAssets, JapaneseName, FallbackKeyword);
		Add(fallbackFontAssets, KoreanName, FallbackKeyword);
		Add(fallbackFontAssets, ThaiName, FallbackKeyword);
	}

	private void AddFontsInThaiOrder(List<FontAsset> fallbackFontAssets)
	{
		Add(fallbackFontAssets, ThaiName, DynamicKeyword);
		Add(fallbackFontAssets, JapaneseName, FallbackKeyword);
		Add(fallbackFontAssets, KoreanName, FallbackKeyword);
		Add(fallbackFontAssets, SimplifiedChineseName, FallbackKeyword);
		Add(fallbackFontAssets, TraditionalChineseName, FallbackKeyword);
	}

	private void AddFontsInDefaultOrder(List<FontAsset> fallbackFontAssets)
	{
		Add(fallbackFontAssets, JapaneseName, FallbackKeyword);
		Add(fallbackFontAssets, KoreanName, FallbackKeyword);
		Add(fallbackFontAssets, SimplifiedChineseName, FallbackKeyword);
		Add(fallbackFontAssets, TraditionalChineseName, FallbackKeyword);
		Add(fallbackFontAssets, ThaiName, FallbackKeyword);
	}

	private void Add(List<FontAsset> fallbackFontAssets, string name, string type)
	{
		fallbackFontAssets.Add(_assetLoader.Load<FontAsset>("UI/Fonts/" + name + type));
	}
}
