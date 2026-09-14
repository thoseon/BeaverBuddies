using System;
using Bindito.Unity;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.SoundSystem;

internal class SoundSystem : ISoundSystem, ILoadableSingleton
{
	private readonly AudioClipService _audioClipService;

	private readonly VolumeController _volumeController;

	private readonly IInstantiator _instantiator;

	private readonly RootObjectProvider _rootObjectProvider;

	private readonly SoundEmitterRetriever _soundEmitterRetriever;

	private AudioListener _audioListener;

	public Vector3 ListenerPosition => _audioListener.transform.position;

	public SoundSystem(AudioClipService audioClipService, VolumeController volumeController, IInstantiator instantiator, RootObjectProvider rootObjectProvider, SoundEmitterRetriever soundEmitterRetriever)
	{
		_audioClipService = audioClipService;
		_volumeController = volumeController;
		_instantiator = instantiator;
		_rootObjectProvider = rootObjectProvider;
		_soundEmitterRetriever = soundEmitterRetriever;
	}

	public void Load()
	{
		_audioClipService.LoadAudioClips();
		_audioListener = _rootObjectProvider.CreateRootObject("AudioListener").AddComponent<AudioListener>();
	}

	public void SetListenerPosition(Vector3 position, Quaternion rotation)
	{
		_audioListener.transform.SetPositionAndRotation(position, rotation);
	}

	public void SetMixerVolume(string name, float level)
	{
		_volumeController.SetVolume(name, level);
	}

	public float GetMixerVolume(string name)
	{
		return _volumeController.GetVolume(name);
	}

	public void SetMasterVolume(float level)
	{
		_volumeController.SetMasterVolume(level);
	}

	public void SetMusicVolume(float level)
	{
		_volumeController.SetMusicVolume(level);
	}

	public void SetUIVolume(float level)
	{
		_volumeController.SetUIVolume(level);
	}

	public void SetEnvironmentVolume(float level)
	{
		_volumeController.SetEnvironmentVolume(level);
	}

	public void PlaySound2D(GameObject emitter, string soundName, int priority)
	{
		_soundEmitterRetriever.GetSoundEmitter(emitter).Start2D(soundName, priority);
	}

	public void PlaySound2D(GameObject emitter, string soundName, int priority, float delay, Action callback)
	{
		_soundEmitterRetriever.GetSoundEmitter(emitter).Start2D(soundName, priority, delay, callback);
	}

	public void PlaySound3D(GameObject emitter, string soundName, int priority)
	{
		_soundEmitterRetriever.GetSoundEmitter(emitter).Start3D(soundName, priority);
	}

	public void PlaySound3D(GameObject emitter, string soundName, int priority, Action callback)
	{
		_soundEmitterRetriever.GetSoundEmitter(emitter).Start3D(soundName, priority, callback);
	}

	public void AddLimitedAreaSound(Transform parent, string soundName, int priority, int cutoffDistance, string customMixer)
	{
		_instantiator.AddComponent<LimitedAreaSoundController>(parent.gameObject).Initialize(soundName, priority, cutoffDistance, customMixer);
	}

	public void AddAreaEmitter(Transform parent, GameObject emitter)
	{
		parent.GetComponent<LimitedAreaSoundController>().Add(emitter);
	}

	public void RemoveAreaEmitter(Transform parent, GameObject emitter)
	{
		parent.GetComponent<LimitedAreaSoundController>().Remove(emitter);
	}

	public void AddLargeAreaSound(Transform parent, IEmitterMap emitterMap, string soundName, int priority, int cutoffDistance, string customMixer)
	{
		_instantiator.AddComponent<LargeAreaSoundController>(parent.gameObject).Initialize(soundName, emitterMap, priority, cutoffDistance, customMixer);
	}

	public void LoopSingle3DSound(GameObject emitter, string soundName, int priority)
	{
		LoopSingle3DSound(emitter, soundName, priority, Vector3.zero);
	}

	public void LoopSingle3DSound(GameObject emitter, string soundName, int priority, Vector3 soundOffset)
	{
		_soundEmitterRetriever.GetSoundEmitter(emitter).LoopSingle3DSound(soundName, priority, soundOffset);
	}

	public void LoopSingle2DSound(GameObject emitter, string soundName, int priority)
	{
		_soundEmitterRetriever.GetSoundEmitter(emitter).LoopSingle2DSound(soundName, priority);
	}

	public void StopSound(GameObject emitter, string soundName)
	{
		_soundEmitterRetriever.GetSoundEmitter(emitter).Stop(soundName);
	}

	public void SetCustomMixer(GameObject emitter, string soundName, string mixerName)
	{
		_soundEmitterRetriever.GetSoundEmitter(emitter).SetCustomMixer(soundName, mixerName);
	}

	public void InvalidateSounds(GameObject emitter)
	{
		_soundEmitterRetriever.GetSoundEmitter(emitter).InvalidateSounds();
	}
}
