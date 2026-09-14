using System.Collections.Generic;
using Bindito.Core;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.SoundSystem;

internal class LargeAreaSoundController : MonoBehaviour
{
	private AudioSourceFactory _audioSourceFactory;

	private ISoundSystem _soundSystem;

	private AudioMixerGroupRetriever _audioMixerGroupRetriever;

	private IEmitterMap _emitterMap;

	private AudioSource _audioSource;

	private int _cutoffDistance;

	private Vector3 _listenerPosition;

	private readonly Queue<Vector2Int> _tilesToCheck = new Queue<Vector2Int>();

	[Inject]
	public void InjectDependencies(AudioSourceFactory audioSourceFactory, ISoundSystem soundSystem, AudioMixerGroupRetriever audioMixerGroupRetriever)
	{
		_audioSourceFactory = audioSourceFactory;
		_soundSystem = soundSystem;
		_audioMixerGroupRetriever = audioMixerGroupRetriever;
	}

	public void Update()
	{
		if (_listenerPosition != _soundSystem.ListenerPosition)
		{
			UpdateAudioSource();
		}
	}

	public void Initialize(string soundName, IEmitterMap emitterMap, int priority, int cutoffDistance, string customMixer)
	{
		_emitterMap = emitterMap;
		_audioSource = _audioSourceFactory.Create(base.gameObject, soundName, cutoffDistance);
		_audioSource.priority = priority;
		_audioSource.outputAudioMixerGroup = _audioMixerGroupRetriever.GetAudioMixerGroup(customMixer);
		_cutoffDistance = cutoffDistance;
	}

	public void OnDestroy()
	{
		_audioSource.Stop();
	}

	private void UpdateAudioSource()
	{
		_listenerPosition = _soundSystem.ListenerPosition;
		float? num = ClosestEmitterDistance();
		if (num.HasValue)
		{
			float valueOrDefault = num.GetValueOrDefault();
			if (!_audioSource.isPlaying)
			{
				_audioSource.Play();
			}
			_audioSource.volume = Mathf.Clamp01(1f - valueOrDefault / (float)_cutoffDistance);
		}
		else if (_audioSource.isPlaying)
		{
			_audioSource.Stop();
		}
	}

	private float? ClosestEmitterDistance()
	{
		Vector2Int vector2Int = new Vector2(_listenerPosition.x, _listenerPosition.z).FloorToInt();
		_tilesToCheck.Clear();
		_tilesToCheck.Enqueue(vector2Int);
		int num = Mathf.CeilToInt((float)(_cutoffDistance * _cutoffDistance) * 3.14f);
		for (int i = 0; i < num; i++)
		{
			Vector2Int vector2Int2 = _tilesToCheck.Dequeue();
			if (_emitterMap.IsEmitterAt(vector2Int2))
			{
				return Vector2Int.Distance(vector2Int2, vector2Int);
			}
			_tilesToCheck.Enqueue(vector2Int2 + Vector2Int.down);
			_tilesToCheck.Enqueue(vector2Int2 + Vector2Int.left);
			_tilesToCheck.Enqueue(vector2Int2 + Vector2Int.up);
			_tilesToCheck.Enqueue(vector2Int2 + Vector2Int.right);
		}
		return null;
	}
}
