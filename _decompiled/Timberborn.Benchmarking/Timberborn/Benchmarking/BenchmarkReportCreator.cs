using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Timberborn.Buildings;
using Timberborn.Characters;
using Timberborn.Common;
using Timberborn.ConstructionSites;
using Timberborn.EntitySystem;
using Timberborn.FeatureToggleSystem;
using Timberborn.GameSaveRepositorySystem;
using Timberborn.SceneLoading;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.Benchmarking;

internal class BenchmarkReportCreator
{
	private readonly StatisticsCalculator _statisticsCalculator;

	private readonly CharacterPopulation _characterPopulation;

	private readonly EntityComponentRegistry _entityComponentRegistry;

	private readonly ISceneLoader _sceneLoader;

	private readonly ITickService _tickService;

	public BenchmarkReportCreator(StatisticsCalculator statisticsCalculator, CharacterPopulation characterPopulation, EntityComponentRegistry entityComponentRegistry, ISceneLoader sceneLoader, ITickService tickService)
	{
		_statisticsCalculator = statisticsCalculator;
		_characterPopulation = characterPopulation;
		_entityComponentRegistry = entityComponentRegistry;
		_sceneLoader = sceneLoader;
		_tickService = tickService;
	}

	public BenchmarkReport CreateReport(BenchmarkParameters benchmarkParameters, SaveReference saveReference)
	{
		BenchmarkReport.Builder builder = new BenchmarkReport.Builder();
		builder.AddTitle("Benchmark report");
		AddBasicInfo(builder, benchmarkParameters, saveReference);
		AddPerformanceReports(builder, benchmarkParameters);
		AddSystemInfo(builder);
		return builder.Build();
	}

	private void AddBasicInfo(BenchmarkReport.Builder builder, BenchmarkParameters benchmarkParameters, SaveReference saveReference)
	{
		builder.AddEntry("Date and time", GetCurrentDateAndTime()).AddEntry("Mode", GetApplicationMode()).AddEntry("Save", saveReference.ToString())
			.AddEntry("Resolution", Screen.currentResolution)
			.AddEntry("Quality", GetNameOfCurrentQualityLevel())
			.AddEntry("VSync", QualitySettings.vSyncCount)
			.AddEntry("Game speed", $"x{benchmarkParameters.GameSpeed}")
			.AddEntry("Warm up duration", benchmarkParameters.WarmUpLengthInSeconds, $"{benchmarkParameters.WarmUpLengthInSeconds}s")
			.AddEntry("Sampling duration", benchmarkParameters.SamplingLengthInSeconds, $"{benchmarkParameters.SamplingLengthInSeconds}s")
			.AddEntry("Number of characters", _characterPopulation.NumberOfCharacters)
			.AddEntry("Number of buildings", _entityComponentRegistry.GetEnabled<Building>().Count())
			.AddEntry("Number of construction sites", _entityComponentRegistry.GetEnabled<ConstructionSite>().Count())
			.AddEntry("Timestep", _tickService.TickIntervalInSeconds, $"{_tickService.TickIntervalInSeconds:0.000}s")
			.AddEntry("Unity version", Application.unityVersion)
			.AddEntry("Application data path", Application.dataPath)
			.AddEntry("Load time", _sceneLoader.LastLoadTimeMs, $"{_sceneLoader.LastLoadTimeMs}ms")
			.AddEntry("Feature toggles", GetFeatureToggles());
	}

	private void AddPerformanceReports(BenchmarkReport.Builder builder, BenchmarkParameters benchmarkParameters)
	{
		ReadOnlyList<PerformanceSample> performanceSamples = benchmarkParameters.PerformanceSamples;
		AddPerformanceReport(builder, "Delta time", performanceSamples.Select((PerformanceSample s) => s.DeltaInSeconds), showFps: true);
		if (benchmarkParameters.DetailedSamplesAvailable)
		{
			AddDetailedPerformanceReports(builder, performanceSamples);
		}
	}

	private void AddDetailedPerformanceReports(BenchmarkReport.Builder builder, IReadOnlyList<PerformanceSample> performanceSamples)
	{
		AddPerformanceReport(builder, "CPU Total", performanceSamples.Select((PerformanceSample s) => s.CpuTotalTime), showFps: true);
		AddPerformanceReport(builder, "CPU Main", performanceSamples.Select((PerformanceSample s) => s.CpuMainThreadTime), showFps: false);
		AddPerformanceReport(builder, "CPU Render", performanceSamples.Select((PerformanceSample s) => s.CpuRenderThreadTime), showFps: false);
		AddPerformanceReport(builder, "CPU Wait", performanceSamples.Select((PerformanceSample s) => s.CpuWaitTime), showFps: false);
		AddPerformanceReport(builder, "GPU", performanceSamples.Select((PerformanceSample s) => s.GpuTime), showFps: false);
	}

	private void AddPerformanceReport(BenchmarkReport.Builder builder, string sectionName, IEnumerable<float> values, bool showFps)
	{
		List<float> list = values.ToList();
		builder.AddSection(sectionName);
		AddPercentile(builder, list, 95f, showFps);
		AddPercentile(builder, list, 98f, showFps);
		AddPercentile(builder, list, 99.5f, showFps);
		AddPercentile(builder, list, 99.9f, showFps);
		AddFrameLength(builder, "Median", _statisticsCalculator.Median(list), showFps);
		AddFrameLength(builder, "Average", list.Average(), showFps);
		AddFrameLength(builder, "Min", list.Min(), showFps);
		AddFrameLength(builder, "Max", list.Max(), showFps);
	}

	private void AddPercentile(BenchmarkReport.Builder builder, IEnumerable<float> values, float percentile, bool showFps)
	{
		float frameLengthInSeconds = _statisticsCalculator.Percentile(values, percentile);
		AddFrameLength(builder, $"{percentile:0.0}th", frameLengthInSeconds, showFps);
	}

	private static void AddFrameLength(BenchmarkReport.Builder builder, string name, float frameLengthInSeconds, bool showFps)
	{
		builder.AddEntry(name, frameLengthInSeconds * 1000f, FormatFrameLength(frameLengthInSeconds, showFps) ?? "");
	}

	private static string FormatFrameLength(float frameLengthInSeconds, bool showFps)
	{
		float num = frameLengthInSeconds * 1000f;
		string text = $"{num:0.0} ms";
		if (!showFps)
		{
			return text;
		}
		return $"{text} ({1f / frameLengthInSeconds:0.0} fps)";
	}

	private static void AddSystemInfo(BenchmarkReport.Builder builder)
	{
		builder.AddSection("System").AddEntry("CPU", SystemInfo.processorType).AddEntry("CPU manufacturer", SystemInfo.processorManufacturer)
			.AddEntry("CPU model", SystemInfo.processorModel)
			.AddEntry("CPU count", SystemInfo.processorCount)
			.AddEntry("CPU frequency", SystemInfo.processorFrequency)
			.AddEntry("GPU", SystemInfo.graphicsDeviceName)
			.AddEntry("GPU memory", $"{SystemInfo.graphicsMemorySize}MB")
			.AddEntry("RAM", $"{SystemInfo.systemMemorySize}MB");
	}

	private static string GetCurrentDateAndTime()
	{
		return DateTime.Now.ToString("s", CultureInfo.InvariantCulture);
	}

	private static string GetNameOfCurrentQualityLevel()
	{
		return QualitySettings.names[QualitySettings.GetQualityLevel()];
	}

	private static string GetApplicationMode()
	{
		if (!Application.isEditor)
		{
			return "Standalone";
		}
		return "Editor";
	}

	private static string GetFeatureToggles()
	{
		return string.Join(", ", FeatureToggleService.GetToggleNames().Where(FeatureToggleService.IsToggleOn));
	}
}
