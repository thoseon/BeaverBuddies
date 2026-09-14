using Bindito.Unity;
using Timberborn.AssetSystem;
using Timberborn.GraphicsQualitySystem;
using Timberborn.RootProviders;
using Timberborn.ScreenSystem;
using Timberborn.SettingsSystem;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Timberborn.Rendering;

public class PostprocessingService : ILoadableSingleton, IPostLoadableSingleton, IUnloadableSingleton
{
	private static readonly GlobalKeyword BloomPropertyId = GlobalKeyword.Create("_BLOOM_ENABLED");

	private static readonly string VolumePrefabPath = "Rendering/Volume";

	private readonly IAssetLoader _assetLoader;

	private readonly IInstantiator _instantiator;

	private readonly RootObjectProvider _rootObjectProvider;

	private readonly GraphicsQualitySettings _graphicsQualitySettings;

	private readonly ScreenSettings _screenSettings;

	private Bloom _bloom;

	private ColorAdjustments _colorAdjustments;

	private float _initialPostExposure;

	public PostprocessingService(IAssetLoader assetLoader, IInstantiator instantiator, RootObjectProvider rootObjectProvider, GraphicsQualitySettings graphicsQualitySettings, ScreenSettings screenSettings)
	{
		_assetLoader = assetLoader;
		_instantiator = instantiator;
		_rootObjectProvider = rootObjectProvider;
		_graphicsQualitySettings = graphicsQualitySettings;
		_screenSettings = screenSettings;
	}

	public void Load()
	{
		GameObject prefab = _assetLoader.Load<GameObject>(VolumePrefabPath);
		GameObject gameObject = _rootObjectProvider.CreateRootObject("PostprocessingService");
		Volume component = _instantiator.Instantiate(prefab, gameObject.transform).GetComponent<Volume>();
		component.profile.TryGet<Bloom>(out _bloom);
		component.profile.TryGet<ColorAdjustments>(out _colorAdjustments);
		_initialPostExposure = _colorAdjustments.postExposure.value;
	}

	public void PostLoad()
	{
		_graphicsQualitySettings.BloomChanged += delegate
		{
			UpdateBloom();
		};
		_screenSettings.BrightnessChanged += OnBrightnessChanged;
		UpdateBloom();
		UpdateBrightness();
	}

	public void Unload()
	{
		_screenSettings.BrightnessChanged -= OnBrightnessChanged;
	}

	private void UpdateBloom()
	{
		_bloom.active = _graphicsQualitySettings.BloomEnabled;
		Shader.SetKeyword(in BloomPropertyId, _bloom.active);
	}

	private void OnBrightnessChanged(object sender, SettingChangedEventArgs<float> e)
	{
		UpdateBrightness();
	}

	private void UpdateBrightness()
	{
		float num = (_screenSettings.Brightness - 1f) * 6f;
		_colorAdjustments.postExposure.value = _initialPostExposure + num;
		_colorAdjustments.postExposure.overrideState = true;
	}
}
