using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.SettingsSystem;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Timberborn.GraphicsQualitySystem;

internal class ShadowQualityGraphicsSettings : ILoadableSingleton
{
	public static readonly ImmutableArray<int> ValidValues = ((IEnumerable<int>)new int[5] { 0, 1, 2, 3, 4 }).ToImmutableArray();

	private readonly GraphicsQualitySettings _graphicsQualitySettings;

	private readonly UniversalRenderPipelineAsset _urpAsset;

	public ShadowQualityGraphicsSettings(GraphicsQualitySettings graphicsQualitySettings)
	{
		_graphicsQualitySettings = graphicsQualitySettings;
		_urpAsset = QualitySettings.renderPipeline as UniversalRenderPipelineAsset;
	}

	public static int GetValueForPreset(GraphicsQualityPreset preset)
	{
		return preset switch
		{
			GraphicsQualityPreset.Ultra => 4, 
			GraphicsQualityPreset.High => 3, 
			GraphicsQualityPreset.Medium => 2, 
			GraphicsQualityPreset.Low => 0, 
			_ => throw new ArgumentException(), 
		};
	}

	public void Load()
	{
		_graphicsQualitySettings.ShadowQualityChanged += delegate(object _, SettingChangedEventArgs<int> args)
		{
			Set(args.Value);
		};
		Set(_graphicsQualitySettings.ShadowQuality);
	}

	private void Set(int value)
	{
		UniversalRenderPipelineAsset urpAsset = _urpAsset;
		urpAsset.mainLightShadowmapResolution = value switch
		{
			0 => 1024, 
			1 => 1024, 
			2 => 2048, 
			3 => 4096, 
			4 => 8192, 
			_ => 4096, 
		};
	}
}
