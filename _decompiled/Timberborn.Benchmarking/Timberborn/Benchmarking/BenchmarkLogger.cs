using System;
using System.Collections.Generic;
using System.IO;
using Timberborn.Common;
using Timberborn.FileSystem;
using Timberborn.GameSaveRepositorySystem;
using Timberborn.GameSaveRuntimeSystem;
using Timberborn.Metrics;
using Timberborn.PlatformUtilities;
using UnityEngine;

namespace Timberborn.Benchmarking;

internal class BenchmarkLogger
{
	private readonly BenchmarkReportCreator _benchmarkReportCreator;

	private readonly GameLoader _gameLoader;

	private readonly GameSaveRepository _gameSaveRepository;

	private readonly PerformanceSampleFormatter _performanceSampleFormatter;

	private readonly IMetricsService _metricsService;

	private readonly IFileService _fileService;

	private static string ApplicationMode
	{
		get
		{
			if (!Application.isEditor)
			{
				return "Standalone";
			}
			return "Editor";
		}
	}

	private static string RootDirectory => Path.Combine(UserDataFolder.Folder, "Benchmarks");

	public BenchmarkLogger(BenchmarkReportCreator benchmarkReportCreator, GameLoader gameLoader, GameSaveRepository gameSaveRepository, PerformanceSampleFormatter performanceSampleFormatter, IMetricsService metricsService, IFileService fileService)
	{
		_benchmarkReportCreator = benchmarkReportCreator;
		_gameLoader = gameLoader;
		_gameSaveRepository = gameSaveRepository;
		_performanceSampleFormatter = performanceSampleFormatter;
		_metricsService = metricsService;
		_fileService = fileService;
	}

	public void LogBenchmark(BenchmarkParameters benchmarkParameters)
	{
		Asserts.FieldIsNotNull(_gameLoader, _gameLoader.LoadedSave, "LoadedSave");
		SaveReference loadedSave = _gameLoader.LoadedSave;
		BenchmarkReport benchmarkReport = _benchmarkReportCreator.CreateReport(benchmarkParameters, loadedSave);
		LogToConsole(benchmarkReport.HumanReadableContent);
		LogToDisk(benchmarkParameters, loadedSave, benchmarkReport.HumanReadableContent);
		LogToArchive(benchmarkReport);
	}

	private static void LogToConsole(string report)
	{
		Debug.Log(report);
	}

	private void LogToDisk(BenchmarkParameters benchmarkParameters, SaveReference saveReference, string report)
	{
		string logName = LogName(benchmarkParameters, saveReference);
		LogToFile(logName, "log", report);
		LogSamplesToFile(benchmarkParameters.PerformanceSamples, logName);
		LogMetricsToFile(logName);
		CopySaveNextToLogFile(saveReference, logName);
	}

	private static string LogName(BenchmarkParameters benchmarkParameters, SaveReference saveReference)
	{
		return $"{saveReference}" + $" {benchmarkParameters.SamplingLengthInSeconds}s" + $" {benchmarkParameters.WarmUpLengthInSeconds}s" + $" x{benchmarkParameters.GameSpeed}" + " " + GetCurrentDateAndTime();
	}

	private static string GetCurrentDateAndTime()
	{
		return DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH\\hmm\\mss\\s");
	}

	private void LogSamplesToFile(IEnumerable<PerformanceSample> samples, string logName)
	{
		string text = _performanceSampleFormatter.FormatAsCsv(samples);
		LogToFile(logName, "csv", text);
	}

	private void LogToFile(string logName, string extension, string text)
	{
		string path = ModeDirectoryFileNameToPath(logName + "." + extension);
		_fileService.WriteTextToFile(path, text);
	}

	private void LogMetricsToFile(string logName)
	{
		_metricsService.WriteCollectedDataToFile(ModeDirectoryFileNameToPath(logName + " metrics"));
	}

	private void CopySaveNextToLogFile(SaveReference saveReference, string logName)
	{
		if (_gameSaveRepository.SaveExists(saveReference))
		{
			string sourceFileName = _gameSaveRepository.SaveNameToFileName(saveReference);
			string destFileName = ModeDirectoryFileNameToPath(logName + ".json");
			File.Copy(sourceFileName, destFileName);
		}
	}

	private void LogToArchive(BenchmarkReport report)
	{
		string text = RootDirectoryFileNameToPath("benchmarks.csv");
		BackupArchiveIfOutdated(report, text);
		if (!File.Exists(text))
		{
			File.WriteAllText(text, report.CsvHeader + "\n");
		}
		File.AppendAllText(text, report.CsvRow + "\n");
	}

	private static void BackupArchiveIfOutdated(BenchmarkReport report, string archivePath)
	{
		if (File.Exists(archivePath))
		{
			string text;
			using (StreamReader streamReader = new StreamReader(archivePath))
			{
				text = streamReader.ReadLine() ?? "";
			}
			if (text != report.CsvHeader)
			{
				string destFileName = RootDirectoryFileNameToPath("benchmarks pre-" + GetCurrentDateAndTime() + ".csv");
				File.Move(archivePath, destFileName);
			}
		}
	}

	private static string ModeDirectoryFileNameToPath(string fileName)
	{
		return Path.Combine(RootDirectory, ApplicationMode, fileName);
	}

	private static string RootDirectoryFileNameToPath(string fileName)
	{
		return Path.Combine(RootDirectory, ApplicationMode + " " + fileName);
	}
}
