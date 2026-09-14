using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Timberborn.GraphicsQualitySystem;

public class WaterQualitySetting : ILoadableSingleton
{
	public static readonly ImmutableArray<int> ValidValues = ((IEnumerable<int>)new int[2] { 0, 1 }).ToImmutableArray();

	private static readonly GlobalKeyword HighQualityWaterEnabledKey = GlobalKeyword.Create("_HIGH_QUALITY_WATER_ENABLED");

	private readonly GraphicsQualitySettings _graphicsQualitySettings;

	private readonly UniversalRenderPipelineAsset _urpAsset;

	public bool HighQualityWaterEnabled => _graphicsQualitySettings.WaterQuality > 0;

	public event EventHandler WaterQualityChanged;

	public WaterQualitySetting(GraphicsQualitySettings graphicsQualitySettings)
	{
		_graphicsQualitySettings = graphicsQualitySettings;
		_urpAsset = QualitySettings.renderPipeline as UniversalRenderPipelineAsset;
	}

	public static int GetValueForPreset(GraphicsQualityPreset preset)
	{
		return preset switch
		{
			GraphicsQualityPreset.Ultra => 1, 
			GraphicsQualityPreset.High => 1, 
			GraphicsQualityPreset.Medium => 1, 
			GraphicsQualityPreset.Low => 0, 
			_ => throw new ArgumentException(), 
		};
	}

	public void Load()
	{
		_graphicsQualitySettings.WaterQualityChanged += delegate
		{
			UpdateQuality();
		};
		UpdateQuality();
	}

	private void UpdateQuality()
	{
		_urpAsset.supportsCameraOpaqueTexture = HighQualityWaterEnabled;
		Shader.SetKeyword(in HighQualityWaterEnabledKey, HighQualityWaterEnabled);
		WaterQualityChanged?.Invoke(this, EventArgs.Empty);
	}
}
