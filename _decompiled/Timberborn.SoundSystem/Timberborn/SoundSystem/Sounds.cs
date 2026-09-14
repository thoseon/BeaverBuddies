using System;
using System.Collections.Generic;
using System.Linq;
using Bindito.Core;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.SoundSystem;

internal class Sounds : MonoBehaviour
{
	private AudioClipService _audioClipService;

	private IRandomNumberGenerator _randomNumberGenerator;

	private AudioSourceFactory _audioSourceFactory;

	private readonly Dictionary<string, List<AudioSource>> _sounds = new Dictionary<string, List<AudioSource>>();

	private readonly Dictionary<string, string> _previousSound = new Dictionary<string, string>();

	private string _customMixerName;

	[Inject]
	public void InjectDependencies(AudioClipService audioClipService, IRandomNumberGenerator randomNumberGenerator, AudioSourceFactory audioSourceFactory)
	{
		_audioClipService = audioClipService;
		_randomNumberGenerator = randomNumberGenerator;
		_audioSourceFactory = audioSourceFactory;
	}

	public AudioSource GetRandomSound(string soundName, Vector3 offset)
	{
		if (!_sounds.ContainsKey(soundName))
		{
			AddSounds(soundName, offset);
		}
		List<AudioSource> list = _sounds[soundName];
		AudioSource listElement = _randomNumberGenerator.GetListElement(list);
		if (list.Count > 1)
		{
			while (_previousSound[soundName] == listElement.clip.name)
			{
				listElement = _randomNumberGenerator.GetListElement(list);
			}
		}
		_previousSound[soundName] = listElement.clip.name;
		return listElement;
	}

	public IEnumerable<AudioSource> GetExistingSounds(string soundName)
	{
		if (_sounds.TryGetValue(soundName, out var value))
		{
			return value;
		}
		return Enumerable.Empty<AudioSource>();
	}

	public void OnDestroy()
	{
		InvalidateSounds();
	}

	public void SetCustomMixer(string customMixerName)
	{
		_customMixerName = customMixerName;
	}

	public void InvalidateSounds()
	{
		_customMixerName = null;
		foreach (KeyValuePair<string, List<AudioSource>> sound in _sounds)
		{
			foreach (AudioSource item in sound.Value)
			{
				item.Stop();
				UnityEngine.Object.Destroy(item);
			}
		}
		_sounds.Clear();
		_previousSound.Clear();
	}

	private void AddSounds(string soundName, Vector3 offset)
	{
		GameObject audioSourceRoot = CreateAudioSourceRoot(soundName, offset);
		CreateAudioSources(soundName, audioSourceRoot);
		if (_sounds[soundName].Count == 0)
		{
			throw new ArgumentException("No sound files for: " + soundName);
		}
	}

	private GameObject CreateAudioSourceRoot(string soundName, Vector3 offset)
	{
		GameObject obj = new GameObject("AudioSource " + soundName);
		obj.transform.parent = base.gameObject.transform;
		obj.transform.localPosition = offset;
		return obj;
	}

	private void CreateAudioSources(string soundName, GameObject audioSourceRoot)
	{
		_sounds[soundName] = new List<AudioSource>();
		_previousSound[soundName] = null;
		foreach (string audioClipName in _audioClipService.GetAudioClipNames(soundName))
		{
			AudioSource item = _audioSourceFactory.Create(audioSourceRoot, audioClipName, _customMixerName);
			_sounds[soundName].Add(item);
		}
	}
}
