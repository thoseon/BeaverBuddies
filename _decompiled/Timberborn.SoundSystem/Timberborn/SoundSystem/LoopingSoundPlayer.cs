using System;
using Bindito.Core;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.SoundSystem;

internal class LoopingSoundPlayer : MonoBehaviour
{
	private static readonly float MinDelay = 0f;

	private static readonly float MaxDelay = 0.5f;

	private IRandomNumberGenerator _randomNumberGenerator;

	private AudioSourceFader _audioSourceFader;

	private Sounds _sounds;

	private AudioSource _currentlyPlaying;

	[Inject]
	public void InjectDependencies(IRandomNumberGenerator randomNumberGenerator, AudioSourceFader audioSourceFader)
	{
		_randomNumberGenerator = randomNumberGenerator;
		_audioSourceFader = audioSourceFader;
	}

	public void Awake()
	{
		_sounds = GetComponent<Sounds>();
	}

	public void PlayLooping2D(string soundName, int priority)
	{
		PlayLooping(soundName, priority, 0f, Vector3.zero);
	}

	public void PlayLooping3D(string soundName, int priority, Vector3 offset)
	{
		PlayLooping(soundName, priority, 1f, offset);
	}

	public void Stop(string soundName)
	{
		if ((bool)_currentlyPlaying && _currentlyPlaying.name == soundName)
		{
			_currentlyPlaying = null;
		}
	}

	private void ThrowIfAlreadyLooping()
	{
		if (_currentlyPlaying != null)
		{
			throw new InvalidOperationException("This SoundEmitter is currently playing another sound in a loop: " + _currentlyPlaying.name);
		}
	}

	private void PlayLooping(string soundName, int priority, float spatialBlend, Vector3 offset)
	{
		ThrowIfAlreadyLooping();
		_currentlyPlaying = _sounds.GetRandomSound(soundName, offset);
		_currentlyPlaying.name = soundName;
		_currentlyPlaying.priority = priority;
		_currentlyPlaying.spatialBlend = spatialBlend;
		_currentlyPlaying.loop = true;
		_currentlyPlaying.pitch = _randomNumberGenerator.Range(0.9f, 1.1f);
		float delay = _randomNumberGenerator.Range(MinDelay, MaxDelay);
		_currentlyPlaying.PlayDelayed(delay);
		_currentlyPlaying.volume = 0f;
		_audioSourceFader.FadeIn(_currentlyPlaying, delay);
	}
}
