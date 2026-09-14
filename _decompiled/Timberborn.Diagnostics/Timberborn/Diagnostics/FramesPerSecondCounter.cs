using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Diagnostics;

public class FramesPerSecondCounter : IUpdatableSingleton
{
	private readonly struct Sample
	{
		public float Timestamp { get; }

		public float FramesPerSecond { get; }

		public Sample(float timestamp, float framesPerSecond)
		{
			Timestamp = timestamp;
			FramesPerSecond = framesPerSecond;
		}
	}

	private static readonly float SamplingPeriodInSeconds = 3f;

	private static readonly int EstimatedNumberOfSamples = (int)(200f * SamplingPeriodInSeconds);

	private readonly List<Sample> _samples = new List<Sample>(EstimatedNumberOfSamples);

	public float AverageFramesPerSecond { get; private set; }

	public float MinFramesPerSecond { get; private set; }

	public void UpdateSingleton()
	{
		float unscaledTime = Time.unscaledTime;
		Sample item = new Sample(unscaledTime, 1f / Time.unscaledDeltaTime);
		while (!_samples.IsEmpty() && _samples[0].Timestamp < unscaledTime - SamplingPeriodInSeconds)
		{
			_samples.RemoveAt(0);
		}
		_samples.Add(item);
		float num = 0f;
		float num2 = float.PositiveInfinity;
		for (int i = 0; i < _samples.Count; i++)
		{
			float framesPerSecond = _samples[i].FramesPerSecond;
			num += framesPerSecond;
			if (framesPerSecond < num2)
			{
				num2 = framesPerSecond;
			}
		}
		AverageFramesPerSecond = num / (float)_samples.Count;
		MinFramesPerSecond = num2;
	}
}
