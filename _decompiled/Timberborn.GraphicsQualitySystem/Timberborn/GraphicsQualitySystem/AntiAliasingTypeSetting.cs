using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.PlatformUtilities;
using Timberborn.SettingsSystem;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Timberborn.GraphicsQualitySystem;

public class AntiAliasingTypeSetting : ILoadableSingleton
{
	public static readonly ImmutableArray<int> ValidValues = ((IEnumerable<int>)new int[6] { 0, 1, 2, 3, 4, 5 }).ToImmutableArray();

	private readonly GraphicsQualitySettings _graphicsQualitySettings;

	private readonly UniversalRenderPipelineAsset _urpAsset;

	public AntiAliasingTypeSetting(GraphicsQualitySettings graphicsQualitySettings)
	{
		_graphicsQualitySettings = graphicsQualitySettings;
		_urpAsset = QualitySettings.renderPipeline as UniversalRenderPipelineAsset;
	}

	public static AntialiasingType GetValueForPreset(GraphicsQualityPreset preset)
	{
		if (ApplicationPlatform.IsWindows())
		{
			return preset switch
			{
				GraphicsQualityPreset.Ultra => AntialiasingType.MSAAx8, 
				GraphicsQualityPreset.High => AntialiasingType.MSAAx4, 
				GraphicsQualityPreset.Medium => AntialiasingType.FXAA, 
				GraphicsQualityPreset.Low => AntialiasingType.Off, 
				_ => throw new ArgumentException(), 
			};
		}
		return preset switch
		{
			GraphicsQualityPreset.Ultra => AntialiasingType.SMAA, 
			GraphicsQualityPreset.High => AntialiasingType.SMAA, 
			GraphicsQualityPreset.Medium => AntialiasingType.FXAA, 
			GraphicsQualityPreset.Low => AntialiasingType.Off, 
			_ => throw new ArgumentException(), 
		};
	}

	public void Load()
	{
		_graphicsQualitySettings.AntiAliasingTypeChanged += delegate(object _, SettingChangedEventArgs<AntialiasingType> args)
		{
			Set(args.Value);
		};
		Set(_graphicsQualitySettings.AntiAliasingType);
	}

	private void Set(AntialiasingType antialiasingType)
	{
		switch (antialiasingType)
		{
		case AntialiasingType.Off:
		case AntialiasingType.FXAA:
		case AntialiasingType.SMAA:
			_urpAsset.msaaSampleCount = 1;
			break;
		case AntialiasingType.MSAAx2:
			_urpAsset.msaaSampleCount = 2;
			break;
		case AntialiasingType.MSAAx4:
			_urpAsset.msaaSampleCount = 4;
			break;
		case AntialiasingType.MSAAx8:
			_urpAsset.msaaSampleCount = 8;
			break;
		default:
			throw new ArgumentOutOfRangeException("antialiasingType", antialiasingType, null);
		}
	}
}
