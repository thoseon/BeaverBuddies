using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.Benchmarking;

internal class PerformanceSampler : ILateUpdatableSingleton
{
	private static readonly int EstimatedNumberOfSamplesInSecond = 200;

	private readonly Ticker _ticker;

	private List<PerformanceSample> _samples;

	private FrameTimingSampler _frameSampler;

	private double _lengthOfPreviousTickInSeconds;

	private bool _enabled;

	public ReadOnlyList<PerformanceSample> Samples => _samples.AsReadOnlyList();

	public bool DetailedSamplesAvailable => _frameSampler.Enabled;

	public PerformanceSampler(Ticker ticker)
	{
		_ticker = ticker;
	}

	public void LateUpdateSingleton()
	{
		if (_enabled)
		{
			TakeSample();
		}
	}

	public void StartSampling(int samplingLengthInSeconds)
	{
		int capacity = EstimatedNumberOfSamplesInSecond * samplingLengthInSeconds;
		_samples = new List<PerformanceSample>(capacity);
		_frameSampler = new FrameTimingSampler();
		_enabled = true;
	}

	public void StopSampling()
	{
		_enabled = false;
	}

	private void TakeSample()
	{
		_frameSampler.UpdateSamples();
		_samples.Add(new PerformanceSample(Time.unscaledTime, Time.unscaledDeltaTime, _lengthOfPreviousTickInSeconds, _frameSampler.CpuMainThreadTime, _frameSampler.CpuRenderThreadTime, _frameSampler.CpuWaitTime, _frameSampler.CpuTotalTime, _frameSampler.GpuTime));
		_lengthOfPreviousTickInSeconds = _ticker.LengthOfLastTickInSeconds;
	}
}
