using Timberborn.BlueprintSystem;
using Timberborn.CameraSystem;
using Timberborn.SingletonSystem;
using Timberborn.SoundSystem;
using UnityEngine;

namespace Timberborn.CoreSound;

public class CameraHeightVolumeUpdater : ILoadableSingleton, ILateUpdatableSingleton
{
	private readonly ISoundSystem _soundSystem;

	private readonly CameraService _cameraService;

	private readonly ISpecService _specService;

	private CoreSoundSpec _spec;

	public float CameraHeight => _cameraService.NormalizedDefaultZoomLevel;

	public CameraHeightVolumeUpdater(ISoundSystem soundSystem, CameraService cameraService, ISpecService specService)
	{
		_soundSystem = soundSystem;
		_cameraService = cameraService;
		_specService = specService;
	}

	public void Load()
	{
		_spec = _specService.GetSingleSpec<CoreSoundSpec>();
	}

	public void LateUpdateSingleton()
	{
		float distance = Vector3.Distance(_soundSystem.ListenerPosition, _cameraService.Transform.position);
		SetVolume(MixerNames.BuildingMixerNameKey, distance, _spec.MinBuildingFadeDistance, _spec.MaxBuildingFadeDistance);
		SetVolume(MixerNames.AmbientMixerNameKey, CameraHeight, _spec.MinAmbientFade, _spec.MaxAmbientFade);
		SetVolume(MixerNames.WindMixerNameKey, CameraHeight, _spec.MinWindFade, _spec.MaxWindFade);
	}

	private void SetVolume(string mixerName, float distance, float min, float max)
	{
		string text = mixerName + "_Volume";
		_soundSystem.SetMixerVolume(text, GetVolume(text, distance, min, max));
	}

	private float GetVolume(string parameterName, float distance, float min, float max)
	{
		float value = 1f - (distance - min) / (max - min);
		float mixerVolume = _soundSystem.GetMixerVolume(parameterName);
		return Mathf.Clamp01(Mathf.Clamp(value, mixerVolume - 0.05f, mixerVolume + 0.05f));
	}
}
