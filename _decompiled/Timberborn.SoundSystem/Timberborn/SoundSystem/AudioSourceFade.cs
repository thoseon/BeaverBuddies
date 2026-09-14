using UnityEngine;

namespace Timberborn.SoundSystem;

internal readonly struct AudioSourceFade
{
	public float DelayEndTime { get; }

	public float FadeEndTime { get; }

	public float TargetVolume { get; }

	private AudioSourceFade(float delayEndTime, float fadeEndTime, float targetVolume)
	{
		DelayEndTime = delayEndTime;
		FadeEndTime = fadeEndTime;
		TargetVolume = targetVolume;
	}

	public static AudioSourceFade FadeIn(float delay, float fadeLength)
	{
		float unscaledTime = Time.unscaledTime;
		return new AudioSourceFade(unscaledTime + delay, unscaledTime + delay + fadeLength, 1f);
	}

	public static AudioSourceFade FadeOut(float fadeLength)
	{
		return new AudioSourceFade(Time.unscaledTime, Time.unscaledTime + fadeLength, 0f);
	}
}
