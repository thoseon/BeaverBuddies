namespace Timberborn.Benchmarking;

internal readonly struct PerformanceSample
{
	public float TimeInSeconds { get; }

	public float DeltaInSeconds { get; }

	public double TickLengthInSeconds { get; }

	public float CpuMainThreadTime { get; }

	public float CpuRenderThreadTime { get; }

	public float CpuWaitTime { get; }

	public float CpuTotalTime { get; }

	public float GpuTime { get; }

	public PerformanceSample(float timeInSeconds, float deltaInSeconds, double tickLengthInSeconds, float cpuMainThreadTime, float cpuRenderThreadTime, float cpuWaitTime, float cpuTotalTime, float gpuTime)
	{
		TimeInSeconds = timeInSeconds;
		DeltaInSeconds = deltaInSeconds;
		TickLengthInSeconds = tickLengthInSeconds;
		CpuMainThreadTime = cpuMainThreadTime;
		CpuRenderThreadTime = cpuRenderThreadTime;
		CpuWaitTime = cpuWaitTime;
		CpuTotalTime = cpuTotalTime;
		GpuTime = gpuTime;
	}
}
