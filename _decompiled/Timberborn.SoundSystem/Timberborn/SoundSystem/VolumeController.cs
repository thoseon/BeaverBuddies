using UnityEngine;

namespace Timberborn.SoundSystem;

internal class VolumeController
{
	private static readonly int Attenuation = 20;

	private readonly AudioMixerGroupRetriever _audioMixerGroupRetriever;

	public VolumeController(AudioMixerGroupRetriever audioMixerGroupRetriever)
	{
		_audioMixerGroupRetriever = audioMixerGroupRetriever;
	}

	public void SetMasterVolume(float level)
	{
		SetVolume("GameMaster_Volume", level);
	}

	public void SetMusicVolume(float level)
	{
		SetVolume("Music_Volume", level);
	}

	public void SetUIVolume(float level)
	{
		SetVolume("UI_Volume", level);
	}

	public void SetEnvironmentVolume(float level)
	{
		SetVolume("Environment_Volume", level);
	}

	public void SetVolume(string parameter, float level)
	{
		float value = ((level < 0.001f) ? (-80f) : (Mathf.Log(level) * (float)Attenuation));
		_audioMixerGroupRetriever.SetMixerParameter(parameter, value);
	}

	public float GetVolume(string parameter)
	{
		return Mathf.Exp(_audioMixerGroupRetriever.GetMixerParameter(parameter) / (float)Attenuation);
	}
}
