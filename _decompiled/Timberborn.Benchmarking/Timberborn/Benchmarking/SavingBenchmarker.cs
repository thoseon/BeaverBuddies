using System;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.ApplicationLifetime;
using Timberborn.Autosaving;
using Timberborn.CameraSystem;
using Timberborn.GameSaveRuntimeSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Benchmarking;

internal class SavingBenchmarker : IUpdatableSingleton
{
	private readonly Autosaver _autosaver;

	private readonly GameSaver _gameSaver;

	private readonly EdgePanningCameraTargetPicker _edgePanningCameraTargetPicker;

	private readonly StatisticsCalculator _statisticsCalculator;

	private int _warmUpLengthInSeconds;

	private int _saveCount;

	private float _warmUpFinishTimestamp = float.MaxValue;

	private bool _enabled;

	private bool WarmUpFinished => Time.unscaledTime > _warmUpFinishTimestamp;

	public SavingBenchmarker(Autosaver autosaver, GameSaver gameSaver, EdgePanningCameraTargetPicker edgePanningCameraTargetPicker, StatisticsCalculator statisticsCalculator)
	{
		_autosaver = autosaver;
		_gameSaver = gameSaver;
		_edgePanningCameraTargetPicker = edgePanningCameraTargetPicker;
		_statisticsCalculator = statisticsCalculator;
	}

	public void UpdateSingleton()
	{
		if (_enabled)
		{
			UpdateBenchmark();
		}
	}

	public void StartBenchmark(int warmUpLengthInSeconds, int saveCount)
	{
		_warmUpLengthInSeconds = warmUpLengthInSeconds;
		_saveCount = saveCount;
		_edgePanningCameraTargetPicker.Suspend();
		_autosaver.Suspend();
		StartWarmUp();
		_enabled = true;
	}

	private void UpdateBenchmark()
	{
		if (WarmUpFinished)
		{
			Save();
			GameQuitter.Quit();
		}
	}

	private void StartWarmUp()
	{
		_warmUpFinishTimestamp = Time.unscaledTime + (float)_warmUpLengthInSeconds;
		Debug.Log($"Started a warm up, will finish in {_warmUpLengthInSeconds} seconds");
	}

	private void Save()
	{
		Debug.Log("Saving...");
		ImmutableArray<float> immutableArray = (from timeSpan in _gameSaver.BenchmarkSavingToMemory(_saveCount)
			select (float)timeSpan.TotalSeconds).ToImmutableArray();
		Debug.Log("Finished saving benchmark:" + $"\n  Number of saves: {immutableArray.Length}" + $"\n  Average: {immutableArray.Average():0.00}s" + $"\n  Median: {_statisticsCalculator.Median(immutableArray):0.00}s" + $"\n  90th: {_statisticsCalculator.Percentile(immutableArray, 90f):0.00}s" + $"\n  Min: {immutableArray.Min():0.00}s" + $"\n  Max: {immutableArray.Max():0.00}s");
	}
}
