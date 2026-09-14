using Timberborn.ApplicationLifetime;
using Timberborn.Autosaving;
using Timberborn.CameraSystem;
using Timberborn.Common;
using Timberborn.Metrics;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;
using UnityEngine;

namespace Timberborn.Benchmarking;

internal class Benchmarker : IUpdatableSingleton
{
	private readonly PerformanceSampler _performanceSampler;

	private readonly BenchmarkLogger _benchmarkLogger;

	private readonly SpeedManager _speedManager;

	private readonly Autosaver _autosaver;

	private readonly EdgePanningCameraTargetPicker _edgePanningCameraTargetPicker;

	private readonly EventBus _eventBus;

	private readonly IMetricsService _metricsService;

	private int _samplingLengthInSeconds;

	private int _warmUpLengthInSeconds;

	private int _gameSpeed;

	private int _frameCounter;

	private float _warmUpFinishTimestamp = float.MaxValue;

	private float _samplingFinishTimestamp = float.MaxValue;

	private bool _enabled;

	public bool WarmUpInProgress => _warmUpFinishTimestamp != float.MaxValue;

	private bool SamplingInProgress => _samplingFinishTimestamp != float.MaxValue;

	private bool TimeToStartWarmUp => _frameCounter == 2;

	private bool TimeToFinishWarmUp => Time.unscaledTime > _warmUpFinishTimestamp;

	private bool TimeToFinishSampling => Time.unscaledTime > _samplingFinishTimestamp;

	public Benchmarker(PerformanceSampler performanceSampler, BenchmarkLogger benchmarkLogger, SpeedManager speedManager, Autosaver autosaver, EdgePanningCameraTargetPicker edgePanningCameraTargetPicker, EventBus eventBus, IMetricsService metricsService)
	{
		_performanceSampler = performanceSampler;
		_benchmarkLogger = benchmarkLogger;
		_speedManager = speedManager;
		_autosaver = autosaver;
		_edgePanningCameraTargetPicker = edgePanningCameraTargetPicker;
		_eventBus = eventBus;
		_metricsService = metricsService;
	}

	public void UpdateSingleton()
	{
		if (_enabled)
		{
			UpdateBenchmark();
		}
	}

	public float GetTimeLeft()
	{
		float unscaledTime = Time.unscaledTime;
		if (WarmUpInProgress)
		{
			return _warmUpFinishTimestamp - unscaledTime + (float)_samplingLengthInSeconds;
		}
		if (SamplingInProgress)
		{
			return _samplingFinishTimestamp - unscaledTime;
		}
		return 0f;
	}

	public void StartBenchmark(int samplingLengthInSeconds, int warmUpLengthInSeconds, int gameSpeed)
	{
		_samplingLengthInSeconds = samplingLengthInSeconds;
		_warmUpLengthInSeconds = warmUpLengthInSeconds;
		_gameSpeed = gameSpeed;
		_speedManager.ChangeSpeed(gameSpeed);
		_edgePanningCameraTargetPicker.Suspend();
		_autosaver.Suspend();
		_eventBus.Post(new BenchmarkStartedEvent());
		_enabled = true;
	}

	private void UpdateBenchmark()
	{
		_frameCounter++;
		if (TimeToStartWarmUp)
		{
			StartWarmUp();
		}
		if (TimeToFinishWarmUp)
		{
			FinishWarmUp();
			StartSampling();
			_metricsService.ResetMetrics();
		}
		if (TimeToFinishSampling)
		{
			FinishSampling();
			FinishBenchmark();
		}
	}

	private void StartWarmUp()
	{
		_warmUpFinishTimestamp = Time.unscaledTime + (float)_warmUpLengthInSeconds;
		Debug.Log($"Started a warm up, will finish in {_warmUpLengthInSeconds} seconds");
	}

	private void FinishWarmUp()
	{
		_warmUpFinishTimestamp = float.MaxValue;
		Debug.Log("Finished a warm up");
	}

	private void StartSampling()
	{
		_samplingFinishTimestamp = Time.unscaledTime + (float)_samplingLengthInSeconds;
		_performanceSampler.StartSampling(_samplingLengthInSeconds);
		Debug.Log($"Started sampling, will finish in {_samplingLengthInSeconds} seconds");
	}

	private void FinishSampling()
	{
		_samplingFinishTimestamp = float.MaxValue;
		_performanceSampler.StopSampling();
		Debug.Log("Finished sampling");
	}

	private void FinishBenchmark()
	{
		ReadOnlyList<PerformanceSample> samples = _performanceSampler.Samples;
		bool detailedSamplesAvailable = _performanceSampler.DetailedSamplesAvailable;
		BenchmarkParameters benchmarkParameters = new BenchmarkParameters(_samplingLengthInSeconds, _warmUpLengthInSeconds, _gameSpeed, samples, detailedSamplesAvailable);
		_benchmarkLogger.LogBenchmark(benchmarkParameters);
		GameQuitter.Quit();
	}
}
