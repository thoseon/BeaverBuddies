using UnityEngine;

namespace Timberborn.Benchmarking;

public class FrameTimingSampler
{
	private readonly FrameTiming[] _frameTimings = new FrameTiming[1];

	public float CpuMainThreadTime { get; private set; }

	public float CpuRenderThreadTime { get; private set; }

	public float CpuWaitTime { get; private set; }

	public float CpuTotalTime { get; private set; }

	public float GpuTime { get; private set; }

	public bool Enabled => FrameTimingManager.IsFeatureEnabled();

	public void UpdateSamples()
	{
		if (Enabled)
		{
			FrameTimingManager.CaptureFrameTimings();
			if (FrameTimingManager.GetLatestTimings(1u, _frameTimings) != 0)
			{
				FrameTiming frameTiming = _frameTimings[0];
				CpuMainThreadTime = 0.001f * (float)frameTiming.cpuMainThreadFrameTime;
				CpuRenderThreadTime = 0.001f * (float)frameTiming.cpuRenderThreadFrameTime;
				CpuWaitTime = 0.001f * (float)frameTiming.cpuMainThreadPresentWaitTime;
				CpuTotalTime = 0.001f * (float)frameTiming.cpuFrameTime;
				GpuTime = 0.001f * (float)frameTiming.gpuFrameTime;
			}
		}
	}
}
