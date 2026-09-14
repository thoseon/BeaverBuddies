using Timberborn.SettingsSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.GraphicsQualitySystem;

internal class AnisotropicFilteringSetting : ILoadableSingleton
{
	private readonly GraphicsQualitySettings _graphicsQualitySettings;

	private AnisotropicFiltering _initialFiltering;

	private AnisotropicFilteringSetting(GraphicsQualitySettings graphicsQualitySettings)
	{
		_graphicsQualitySettings = graphicsQualitySettings;
	}

	public static bool GetValueForPreset(GraphicsQualityPreset preset)
	{
		return preset != GraphicsQualityPreset.Low;
	}

	public void Load()
	{
		_graphicsQualitySettings.AnisotropicFilteringQualityChanged += delegate(object _, SettingChangedEventArgs<bool> args)
		{
			Set(args.Value);
		};
		Set(_graphicsQualitySettings.AnisotropicFilteringEnabled);
	}

	private void Set(bool value)
	{
		QualitySettings.anisotropicFiltering = (value ? AnisotropicFiltering.Enable : AnisotropicFiltering.Disable);
	}
}
