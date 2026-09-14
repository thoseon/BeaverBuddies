using System.Collections.Generic;
using System.Text;
using Timberborn.Benchmarking;
using Timberborn.Common;
using Timberborn.DebuggingUI;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.BenchmarkingUI;

internal class BenchmarkDebuggingPanel : ILoadableSingleton, IDebuggingPanel
{
	private static readonly float UpdateInterval = 0.5f;

	private readonly DebuggingPanel _debuggingPanel;

	private readonly List<float> _cpuMainThreadTimes = new List<float>();

	private readonly List<float> _cpuRenderThreadTimes = new List<float>();

	private readonly List<float> _cpuWaitTimes = new List<float>();

	private readonly List<float> _cpuTotalTimes = new List<float>();

	private readonly List<float> _gpuTimes = new List<float>();

	private readonly FrameTimingSampler _frameSampler = new FrameTimingSampler();

	private readonly StringBuilder _description = new StringBuilder();

	private float _lastMeasurementTime;

	public BenchmarkDebuggingPanel(DebuggingPanel debuggingPanel)
	{
		_debuggingPanel = debuggingPanel;
	}

	public void Load()
	{
		_debuggingPanel.AddDebuggingPanel(this, "Performance");
	}

	public string GetText()
	{
		_frameSampler.UpdateSamples();
		_cpuTotalTimes.Add(_frameSampler.CpuTotalTime);
		_cpuMainThreadTimes.Add(_frameSampler.CpuMainThreadTime);
		_cpuRenderThreadTimes.Add(_frameSampler.CpuRenderThreadTime);
		_cpuWaitTimes.Add(_frameSampler.CpuWaitTime);
		_gpuTimes.Add(_frameSampler.GpuTime);
		if (Time.unscaledTime > _lastMeasurementTime + UpdateInterval)
		{
			_lastMeasurementTime = Time.unscaledTime;
			_description.Clear();
			AddTimeValue("CPU (Total)", _cpuTotalTimes);
			AddTimeValue("CPU (Main)", _cpuMainThreadTimes);
			AddTimeValue("CPU (Render)", _cpuRenderThreadTimes);
			AddTimeValue("CPU (Wait)", _cpuWaitTimes);
			AddTimeValue("GPU", _gpuTimes);
			return _description.ToStringWithoutNewLineEnd();
		}
		return null;
	}

	private void AddTimeValue(string text, IList<float> values)
	{
		double num = 1000.0 * GetAverageAndClear(values);
		_description.AppendLine($"{text}: {num:0.0}ms");
	}

	private static double GetAverageAndClear(IList<float> values)
	{
		if (values.Count == 0)
		{
			return 0.0;
		}
		double num = 0.0;
		for (int i = 0; i < values.Count; i++)
		{
			num += (double)values[i];
		}
		double result = num / (double)values.Count;
		values.Clear();
		return result;
	}
}
