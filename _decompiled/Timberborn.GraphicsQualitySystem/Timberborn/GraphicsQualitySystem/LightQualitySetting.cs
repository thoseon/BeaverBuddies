using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Timberborn.SettingsSystem;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Timberborn.GraphicsQualitySystem;

internal class LightQualitySetting : ILoadableSingleton
{
	public static readonly ImmutableArray<int> ValidValues = ((IEnumerable<int>)new int[4] { 0, 4, 6, 8 }).ToImmutableArray();

	private readonly GraphicsQualitySettings _graphicsQualitySettings;

	private readonly UniversalRenderPipelineAsset _urpAsset;

	public LightQualitySetting(GraphicsQualitySettings graphicsQualitySettings)
	{
		_graphicsQualitySettings = graphicsQualitySettings;
		_urpAsset = QualitySettings.renderPipeline as UniversalRenderPipelineAsset;
	}

	public static int GetValueForPreset(GraphicsQualityPreset preset)
	{
		return preset switch
		{
			GraphicsQualityPreset.Ultra => 8, 
			GraphicsQualityPreset.High => 6, 
			GraphicsQualityPreset.Medium => 4, 
			GraphicsQualityPreset.Low => 0, 
			_ => throw new ArgumentException(), 
		};
	}

	public void Load()
	{
		_graphicsQualitySettings.LightQualityChanged += delegate(object _, SettingChangedEventArgs<int> args)
		{
			Set(args.Value);
		};
		Set(_graphicsQualitySettings.LightQuality);
	}

	private static void SetAdditionalLightRenderingMode(LightRenderingMode lightRenderingMode, UniversalRenderPipelineAsset urpAsset)
	{
		FieldInfo field = typeof(UniversalRenderPipelineAsset).GetField("m_AdditionalLightsRenderingMode", BindingFlags.Instance | BindingFlags.NonPublic);
		if (field != null)
		{
			field.SetValue(urpAsset, lightRenderingMode);
		}
	}

	private void Set(int value)
	{
		_urpAsset.maxAdditionalLightsCount = value;
		SetAdditionalLightRenderingMode((value != 0) ? LightRenderingMode.PerPixel : LightRenderingMode.Disabled, _urpAsset);
	}
}
