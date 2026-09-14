using System.Collections.Generic;
using Bindito.Core;
using UnityEngine;

namespace Timberborn.SoundSystem;

internal class LimitedAreaSoundController : MonoBehaviour
{
	private AudioSourceFactory _audioSourceFactory;

	private ISoundSystem _soundSystem;

	private AudioMixerGroupRetriever _audioMixerGroupRetriever;

	private readonly List<GameObject> _stationaryEmitters = new List<GameObject>();

	private AudioSource _audioSource;

	private int _cutoffDistance;

	private bool _dirty;

	private Vector3 _listenerPosition;

	[Inject]
	public void InjectDependencies(AudioSourceFactory audioSourceFactory, ISoundSystem soundSystem, AudioMixerGroupRetriever audioMixerGroupRetriever)
	{
		_audioSourceFactory = audioSourceFactory;
		_soundSystem = soundSystem;
		_audioMixerGroupRetriever = audioMixerGroupRetriever;
	}

	public void Update()
	{
		if (_dirty || _listenerPosition != _soundSystem.ListenerPosition)
		{
			UpdateAudioSource();
		}
	}

	public void Initialize(string soundName, int priority, int cutoffDistance, string customMixer)
	{
		_audioSource = _audioSourceFactory.Create(base.gameObject, soundName, cutoffDistance);
		_audioSource.priority = priority;
		_audioSource.outputAudioMixerGroup = _audioMixerGroupRetriever.GetAudioMixerGroup(customMixer);
		_cutoffDistance = cutoffDistance;
	}

	public void Add(GameObject emitter)
	{
		_stationaryEmitters.Add(emitter);
		_dirty = true;
	}

	public void Remove(GameObject emitter)
	{
		_stationaryEmitters.Remove(emitter);
		_dirty = true;
	}

	public void OnDestroy()
	{
		_audioSource.Stop();
	}

	private void UpdateAudioSource()
	{
		_listenerPosition = _soundSystem.ListenerPosition;
		_dirty = false;
		float num = ClosestEmitterDistance();
		if (num < (float)_cutoffDistance)
		{
			if (!_audioSource.isPlaying)
			{
				_audioSource.Play();
			}
			_audioSource.volume = 1f - num / (float)_cutoffDistance;
		}
		else if (num >= (float)_cutoffDistance && _audioSource.isPlaying)
		{
			_audioSource.Stop();
		}
	}

	private float ClosestEmitterDistance()
	{
		float num = float.MaxValue;
		for (int i = 0; i < _stationaryEmitters.Count; i++)
		{
			float num2 = Vector3.Distance(_stationaryEmitters[i].transform.position, _listenerPosition);
			if (num2 < num)
			{
				num = num2;
			}
		}
		return num;
	}
}
