using System.Collections.Generic;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;
using UnityEngine.Audio;

namespace Timberborn.SoundSystem;

internal class AudioMixerGroupRetriever : ILoadableSingleton
{
	private readonly ISpecService _specService;

	private readonly Dictionary<string, AudioMixerGroup> _audioMixerGroups = new Dictionary<string, AudioMixerGroup>();

	private AudioMixer _audioMixer;

	public AudioMixerGroupRetriever(ISpecService specService)
	{
		_specService = specService;
	}

	public void Load()
	{
		_audioMixer = _specService.GetSingleSpec<AudioMixerGroupRetrieverSpec>().AudioMixer.Asset;
	}

	public AudioMixerGroup GetAudioMixerGroup(string audioMixerGroupName)
	{
		if (!_audioMixerGroups.TryGetValue(audioMixerGroupName, out var value))
		{
			return AddAudioMixerGroup(audioMixerGroupName);
		}
		return value;
	}

	public AudioMixerGroup GetAudioMixerGroupFromSoundName(string soundName)
	{
		string[] array = soundName.Split('.');
		return GetAudioMixerGroup(array[0]);
	}

	public void SetMixerParameter(string parameterName, float value)
	{
		_audioMixer.SetFloat(parameterName, value);
	}

	public float GetMixerParameter(string parameterName)
	{
		if (_audioMixer.GetFloat(parameterName, out var value))
		{
			return value;
		}
		return 1f;
	}

	private AudioMixerGroup AddAudioMixerGroup(string audioMixerGroupName)
	{
		AudioMixerGroup audioMixerGroup = _audioMixer.FindMatchingGroups(audioMixerGroupName).Single((AudioMixerGroup group) => group.name == audioMixerGroupName);
		_audioMixerGroups.Add(audioMixerGroupName, audioMixerGroup);
		return audioMixerGroup;
	}
}
