using System;
using System.Collections.Generic;
using Bindito.Core;
using UnityEngine;
using UnityEngine.Audio;

namespace Timberborn.SoundSystem;

internal class SoundEmitter : MonoBehaviour
{
	private class CallbackSound
	{
		public Action Action { get; }

		public float PreviousTime { get; set; }

		public CallbackSound(Action action)
		{
			Action = action;
		}
	}

	private AudioMixerGroupRetriever _audioMixerGroupRetriever;

	private AudioSourceFader _audioSourceFader;

	private Sounds _sounds;

	private readonly Dictionary<AudioSource, CallbackSound> _callbackSounds = new Dictionary<AudioSource, CallbackSound>();

	private LoopingSoundPlayer _loopingSoundPlayer;

	private readonly List<AudioSource> _callbackSoundsToProcess = new List<AudioSource>();

	[Inject]
	public void InjectDependencies(AudioMixerGroupRetriever audioMixerGroupRetriever, AudioSourceFader audioSourceFader)
	{
		_audioMixerGroupRetriever = audioMixerGroupRetriever;
		_audioSourceFader = audioSourceFader;
	}

	public void Awake()
	{
		_sounds = GetComponent<Sounds>();
		_loopingSoundPlayer = GetComponent<LoopingSoundPlayer>();
		base.enabled = false;
	}

	public void Update()
	{
		ProcessCallbackSounds();
	}

	public void Start2D(string soundName, int priority)
	{
		StartSound(soundName, priority, 0f, 0f);
	}

	public void Start2D(string soundName, int priority, float delay, Action callback)
	{
		AudioSource key = StartSound(soundName, priority, 0f, delay);
		_callbackSounds[key] = new CallbackSound(callback);
		base.enabled = true;
	}

	public void Start3D(string soundName, int priority)
	{
		StartSound(soundName, priority, 1f, 0f);
	}

	public void Start3D(string soundName, int priority, Action callback)
	{
		AudioSource key = StartSound(soundName, priority, 1f, 0f);
		_callbackSounds[key] = new CallbackSound(callback);
		base.enabled = true;
	}

	public void LoopSingle2DSound(string soundName, int priority)
	{
		_loopingSoundPlayer.PlayLooping2D(soundName, priority);
	}

	public void LoopSingle3DSound(string soundName, int priority, Vector3 offset)
	{
		_loopingSoundPlayer.PlayLooping3D(soundName, priority, offset);
	}

	public void Stop(string soundName)
	{
		foreach (AudioSource existingSound in _sounds.GetExistingSounds(soundName))
		{
			if (existingSound.isPlaying)
			{
				_audioSourceFader.FadeOut(existingSound);
			}
			RemoveCallbackSound(existingSound);
		}
		_loopingSoundPlayer.Stop(soundName);
	}

	public void SetCustomMixer(string soundName, string customMixerName)
	{
		AudioMixerGroup audioMixerGroup = _audioMixerGroupRetriever.GetAudioMixerGroup(customMixerName);
		foreach (AudioSource existingSound in _sounds.GetExistingSounds(soundName))
		{
			existingSound.outputAudioMixerGroup = audioMixerGroup;
		}
		_sounds.SetCustomMixer(customMixerName);
	}

	public void InvalidateSounds()
	{
		_sounds.InvalidateSounds();
		_callbackSounds.Clear();
		_callbackSoundsToProcess.Clear();
	}

	private void ProcessCallbackSounds()
	{
		_callbackSoundsToProcess.AddRange(_callbackSounds.Keys);
		foreach (AudioSource item in _callbackSoundsToProcess)
		{
			CallbackSound callbackSound = _callbackSounds[item];
			float time = item.time;
			if (time < callbackSound.PreviousTime)
			{
				RemoveCallbackSound(item);
				callbackSound.Action();
			}
			else
			{
				callbackSound.PreviousTime = time;
			}
		}
		_callbackSoundsToProcess.Clear();
	}

	private AudioSource StartSound(string soundName, int priority, float spatialBlend, float delay)
	{
		AudioSource randomSound = _sounds.GetRandomSound(soundName, Vector3.zero);
		_audioSourceFader.RemoveFaders(randomSound);
		randomSound.priority = priority;
		randomSound.spatialBlend = spatialBlend;
		randomSound.volume = 1f;
		randomSound.pitch = 1f;
		randomSound.loop = false;
		randomSound.PlayDelayed(delay);
		return randomSound;
	}

	private void RemoveCallbackSound(AudioSource audioSource)
	{
		_callbackSounds.Remove(audioSource);
		if (_callbackSounds.Count == 0)
		{
			base.enabled = false;
		}
	}
}
