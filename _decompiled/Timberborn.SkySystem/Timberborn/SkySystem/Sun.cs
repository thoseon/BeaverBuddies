using System;
using Timberborn.BlueprintSystem;
using Timberborn.CameraSystem;
using Timberborn.GraphicsQualitySystem;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.Rendering;

namespace Timberborn.SkySystem;

public class Sun : ILoadableSingleton, ILateUpdatableSingleton
{
	private readonly CameraService _cameraService;

	private readonly DayStageCycle _dayStageCycle;

	private readonly ISpecService _specService;

	private readonly GraphicsQualitySettings _graphicsQualitySettings;

	private readonly RootObjectProvider _rootObjectProvider;

	private Light _sun;

	private SunSpec _sunSpec;

	private bool _rotationStopped;

	public bool Fog
	{
		get
		{
			return RenderSettings.fog;
		}
		set
		{
			RenderSettings.fog = value;
		}
	}

	public Transform Transform => _sun.transform;

	public Sun(CameraService cameraService, DayStageCycle dayStageCycle, ISpecService specService, GraphicsQualitySettings graphicsQualitySettings, RootObjectProvider rootObjectProvider)
	{
		_cameraService = cameraService;
		_dayStageCycle = dayStageCycle;
		_specService = specService;
		_graphicsQualitySettings = graphicsQualitySettings;
		_rootObjectProvider = rootObjectProvider;
	}

	public void Load()
	{
		RenderSettings.ambientMode = AmbientMode.Trilight;
		GameObject gameObject = _rootObjectProvider.CreateRootObject("Sun");
		_sunSpec = _specService.GetSingleSpec<SunSpec>();
		_sun = UnityEngine.Object.Instantiate(_sunSpec.SunPrefab.Asset, gameObject.transform);
		_graphicsQualitySettings.ShadowQualityChanged += delegate
		{
			UpdateShadows();
		};
		UpdateShadows();
		UpdateColorsAndRotation();
	}

	public void LateUpdateSingleton()
	{
		UpdateColorsAndRotation();
	}

	public void ToggleSunRotation()
	{
		_rotationStopped = !_rotationStopped;
	}

	public float GetCameraYAngle(Transform cameraTransform)
	{
		return cameraTransform.localRotation.eulerAngles.y + _sunSpec.RotateWithCameraOffset;
	}

	private void UpdateShadows()
	{
		Light sun = _sun;
		LightShadows shadows = ((_graphicsQualitySettings.ShadowQuality != 0) ? LightShadows.Soft : LightShadows.None);
		sun.shadows = shadows;
	}

	private void UpdateColorsAndRotation()
	{
		DayStageTransition currentTransition = _dayStageCycle.GetCurrentTransition();
		UpdateColors(currentTransition);
		UpdateRotation(currentTransition);
	}

	private void UpdateRotation(DayStageTransition dayStageTransition)
	{
		DayStageColorsSpec dayStageColorsSpec = DayStageColors(dayStageTransition.CurrentDayStage);
		DayStageColorsSpec dayStageColorsSpec2 = DayStageColors(dayStageTransition.NextDayStage);
		float x = Mathf.Lerp(t: dayStageTransition.TransitionProgress, a: dayStageColorsSpec.SunXAngle, b: dayStageColorsSpec2.SunXAngle);
		float y = (_rotationStopped ? _sun.transform.localRotation.eulerAngles.y : GetCameraYAngle(_cameraService.Transform));
		_sun.transform.localRotation = Quaternion.Euler(x, y, _sun.transform.localRotation.eulerAngles.z);
	}

	private void UpdateColors(DayStageTransition dayStageTransition)
	{
		DayStageColorsSpec dayStageColorsSpec = DayStageColors(dayStageTransition.CurrentDayStage);
		DayStageColorsSpec dayStageColorsSpec2 = DayStageColors(dayStageTransition.NextDayStage);
		float transitionProgress = dayStageTransition.TransitionProgress;
		_sun.color = Color.Lerp(dayStageColorsSpec.SunColor, dayStageColorsSpec2.SunColor, transitionProgress);
		_sun.intensity = Mathf.Lerp(dayStageColorsSpec.SunIntensity, dayStageColorsSpec2.SunIntensity, transitionProgress);
		_sun.shadowStrength = Mathf.Lerp(dayStageColorsSpec.ShadowStrength, dayStageColorsSpec2.ShadowStrength, transitionProgress);
		RenderSettings.ambientSkyColor = Color.Lerp(dayStageColorsSpec.AmbientSkyColor, dayStageColorsSpec2.AmbientSkyColor, transitionProgress);
		RenderSettings.ambientEquatorColor = Color.Lerp(dayStageColorsSpec.AmbientEquatorColor, dayStageColorsSpec2.AmbientEquatorColor, transitionProgress);
		RenderSettings.ambientGroundColor = Color.Lerp(dayStageColorsSpec.AmbientGroundColor, dayStageColorsSpec2.AmbientGroundColor, transitionProgress);
		RenderSettings.reflectionIntensity = Mathf.Lerp(dayStageColorsSpec.ReflectionsIntensity, dayStageColorsSpec2.ReflectionsIntensity, transitionProgress);
		FogSettingsSpec fogSettings = GetFogSettings(dayStageTransition.CurrentDayStageHazardousWeatherId, dayStageColorsSpec);
		FogSettingsSpec fogSettings2 = GetFogSettings(dayStageTransition.NextDayStageHazardousWeatherId, dayStageColorsSpec2);
		RenderSettings.fogColor = Color.Lerp(fogSettings.FogColor, fogSettings2.FogColor, transitionProgress);
		RenderSettings.fogDensity = Mathf.Lerp(fogSettings.FogDensity, fogSettings2.FogDensity, transitionProgress);
	}

	private DayStageColorsSpec DayStageColors(DayStage dayStage)
	{
		return dayStage switch
		{
			DayStage.Sunrise => _sunSpec.SunriseColors, 
			DayStage.Day => _sunSpec.DayColors, 
			DayStage.Sunset => _sunSpec.SunsetColors, 
			DayStage.Night => _sunSpec.NightColors, 
			_ => throw new ArgumentOutOfRangeException("dayStage", dayStage, null), 
		};
	}

	private static FogSettingsSpec GetFogSettings(string hazardousWeatherId, DayStageColorsSpec dayStageColorsSpec)
	{
		if (string.IsNullOrEmpty(hazardousWeatherId))
		{
			return dayStageColorsSpec.TemperateWeatherFog;
		}
		foreach (HazardousWeatherFogSettingsSpec hazardousWeatherFog in dayStageColorsSpec.HazardousWeatherFogs)
		{
			if (hazardousWeatherFog.HazardousWeatherId == hazardousWeatherId)
			{
				return hazardousWeatherFog.FogSettings;
			}
		}
		throw new ArgumentException("Weather fog settings not found for " + hazardousWeatherId);
	}
}
