using System;
using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.SkySystem;

internal class SkyboxPositioner : ILoadableSingleton, IUpdatableSingleton, IUnloadableSingleton
{
	private static readonly int DayProgressProperty = Shader.PropertyToID("_DayProgress");

	private readonly DayStageCycle _dayStageCycle;

	private readonly ISpecService _specService;

	private SkyboxPositionerSpec _skyboxPositionerSpec;

	public Material SkyboxMaterial { get; private set; }

	public SkyboxPositioner(DayStageCycle dayStageCycle, ISpecService specService)
	{
		_dayStageCycle = dayStageCycle;
		_specService = specService;
	}

	public void Load()
	{
		_skyboxPositionerSpec = _specService.GetSingleSpec<SkyboxPositionerSpec>();
		SkyboxMaterial = new Material(_skyboxPositionerSpec.Skybox.Asset);
		UpdateDayProgress();
	}

	public void UpdateSingleton()
	{
		UpdateDayProgress();
	}

	public void Unload()
	{
		UnityEngine.Object.Destroy(SkyboxMaterial);
	}

	private void UpdateDayProgress()
	{
		DayStageTransition currentTransition = _dayStageCycle.GetCurrentTransition();
		float value = LerpDayProgressWrappingFrom1To0(DayProgress(currentTransition.CurrentDayStage), DayProgress(currentTransition.NextDayStage), currentTransition.TransitionProgress);
		SkyboxMaterial.SetFloat(DayProgressProperty, value);
	}

	private static float LerpDayProgressWrappingFrom1To0(float previousValue, float nextValue, float transitionProgress)
	{
		if (previousValue > nextValue)
		{
			nextValue++;
		}
		float num = Mathf.Lerp(previousValue, nextValue, transitionProgress);
		if (num > 1f)
		{
			num--;
		}
		return num;
	}

	private float DayProgress(DayStage dayStage)
	{
		return dayStage switch
		{
			DayStage.Sunrise => _skyboxPositionerSpec.DayProgressSunrise, 
			DayStage.Day => _skyboxPositionerSpec.DayProgressDay, 
			DayStage.Sunset => _skyboxPositionerSpec.DayProgressSunset, 
			DayStage.Night => _skyboxPositionerSpec.DayProgressNight, 
			_ => throw new ArgumentOutOfRangeException("dayStage", dayStage, null), 
		};
	}
}
