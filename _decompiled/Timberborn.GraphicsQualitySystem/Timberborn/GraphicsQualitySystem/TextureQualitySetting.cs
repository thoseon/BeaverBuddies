using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.SettingsSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.GraphicsQualitySystem;

internal class TextureQualitySetting : ILoadableSingleton
{
	public static readonly ImmutableArray<int> ValidValues = ((IEnumerable<int>)new int[3] { 0, 1, 2 }).ToImmutableArray();

	private readonly GraphicsQualitySettings _graphicsQualitySettings;

	public TextureQualitySetting(GraphicsQualitySettings graphicsQualitySettings)
	{
		_graphicsQualitySettings = graphicsQualitySettings;
	}

	public static int GetValueForPreset(GraphicsQualityPreset preset)
	{
		return preset switch
		{
			GraphicsQualityPreset.Ultra => 0, 
			GraphicsQualityPreset.High => 0, 
			GraphicsQualityPreset.Medium => 1, 
			GraphicsQualityPreset.Low => 2, 
			_ => throw new ArgumentException(), 
		};
	}

	public void Load()
	{
		_graphicsQualitySettings.TextureQualityChanged += delegate(object _, SettingChangedEventArgs<int> args)
		{
			Set(args.Value);
		};
		Set(_graphicsQualitySettings.TextureQuality);
	}

	private static void Set(int value)
	{
		QualitySettings.globalTextureMipmapLimit = value;
	}
}
