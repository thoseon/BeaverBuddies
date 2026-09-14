using System;
using Timberborn.GraphicsQualitySystem;
using Timberborn.SettingsSystem;
using Timberborn.SingletonSystem;
using UnityEngine.Rendering.Universal;

namespace Timberborn.CameraSystem;

internal class CameraAntiAliasing : ILoadableSingleton
{
	private readonly GraphicsQualitySettings _graphicsQualitySettings;

	private readonly CameraService _cameraService;

	public CameraAntiAliasing(GraphicsQualitySettings graphicsQualitySettings, CameraService cameraService)
	{
		_graphicsQualitySettings = graphicsQualitySettings;
		_cameraService = cameraService;
	}

	public void Load()
	{
		UpdateAntiAliasing(_graphicsQualitySettings.AntiAliasingType);
		_graphicsQualitySettings.AntiAliasingTypeChanged += delegate(object _, SettingChangedEventArgs<AntialiasingType> antiAliasingType)
		{
			UpdateAntiAliasing(antiAliasingType.Value);
		};
	}

	private void UpdateAntiAliasing(AntialiasingType antiAliasingType)
	{
		UniversalAdditionalCameraData component = _cameraService.Transform.GetComponent<UniversalAdditionalCameraData>();
		switch (antiAliasingType)
		{
		case AntialiasingType.Off:
		case AntialiasingType.MSAAx2:
		case AntialiasingType.MSAAx4:
		case AntialiasingType.MSAAx8:
			component.antialiasing = AntialiasingMode.None;
			break;
		case AntialiasingType.FXAA:
			component.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
			break;
		case AntialiasingType.SMAA:
			component.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
			break;
		default:
			throw new ArgumentOutOfRangeException("antiAliasingType", antiAliasingType, null);
		}
	}
}
