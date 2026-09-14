using UnityEngine;

namespace Timberborn.SoundSystem;

internal class AudioSourceFactory
{
	private static readonly int SoundCutOffDistance = 10;

	private readonly AudioClipService _audioClipService;

	private readonly AudioMixerGroupRetriever _audioMixerGroupRetriever;

	public AudioSourceFactory(AudioClipService audioClipService, AudioMixerGroupRetriever audioMixerGroupRetriever)
	{
		_audioClipService = audioClipService;
		_audioMixerGroupRetriever = audioMixerGroupRetriever;
	}

	public AudioSource Create(GameObject emitter, string soundName, string mixerName)
	{
		return Create(emitter, soundName, SoundCutOffDistance, mixerName);
	}

	public AudioSource Create(GameObject emitter, string soundName, int cutoffDistance, string mixerName = null)
	{
		AudioClip audioClip = _audioClipService.GetAudioClip(soundName);
		AudioSource audioSource = emitter.AddComponent<AudioSource>();
		audioSource.clip = audioClip;
		audioSource.playOnAwake = false;
		audioSource.dopplerLevel = 0f;
		audioSource.minDistance = 2f;
		audioSource.maxDistance = cutoffDistance;
		audioSource.rolloffMode = AudioRolloffMode.Linear;
		audioSource.outputAudioMixerGroup = (string.IsNullOrWhiteSpace(mixerName) ? _audioMixerGroupRetriever.GetAudioMixerGroupFromSoundName(soundName) : _audioMixerGroupRetriever.GetAudioMixerGroup(mixerName));
		return audioSource;
	}
}
