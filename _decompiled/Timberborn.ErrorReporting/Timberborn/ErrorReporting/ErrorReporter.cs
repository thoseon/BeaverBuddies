using System;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Text;
using Timberborn.PlatformUtilities;
using Timberborn.Versioning;
using UnityEngine;

namespace Timberborn.ErrorReporting;

public class ErrorReporter : MonoBehaviour
{
	public static readonly string ErrorReportsFolder = Path.Combine(UserDataFolder.Folder, "Error reports");

	public static string LogString;

	public static string StackTrace;

	public static byte[] ExceptionSave;

	private static string reportTimestamp;

	public static string ReportFilePath { get; private set; }

	public static bool ErrorReported
	{
		get
		{
			if (string.IsNullOrWhiteSpace(LogString))
			{
				return !string.IsNullOrWhiteSpace(StackTrace);
			}
			return true;
		}
	}

	public static void CreateErrorReport()
	{
		ReportFilePath = null;
		reportTimestamp = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd-HH\\hmm\\mss\\s");
		string text = (Application.isEditor ? "-editor" : "");
		string path = "error-report-" + reportTimestamp + text + ".zip";
		string text2 = Path.Combine(ErrorReportsFolder, path);
		Debug.Log("Creating an error report: " + text2);
		try
		{
			Directory.CreateDirectory(ErrorReportsFolder);
			using FileStream stream = new FileStream(text2, FileMode.CreateNew);
			using ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Update);
			AddVersionEntry(archive);
			AddExceptionEntry(archive);
			AddStartingItemEntry(archive);
			AddSaveEntry(archive);
			AddPlayerLogEntry(archive);
		}
		catch (Exception arg)
		{
			Debug.LogWarning($"Failed to create an error report: {arg}");
		}
		ReportFilePath = text2;
	}

	public static void AddCommentToReport(string comment)
	{
		AddPlainTextEntryToExistingReport("3 Comment", comment);
	}

	public static void AddEmailToReport(string email)
	{
		AddPlainTextEntryToExistingReport("4 Email", email);
	}

	private static void AddVersionEntry(ZipArchive archive)
	{
		AddPlainTextEntry(archive, "0 Version", GameVersions.CurrentVersion.Formatted);
	}

	private static void AddExceptionEntry(ZipArchive archive)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (!string.IsNullOrWhiteSpace(LogString))
		{
			stringBuilder.AppendLine(LogString);
		}
		if (!string.IsNullOrWhiteSpace(StackTrace))
		{
			stringBuilder.AppendLine(StackTrace);
		}
		AddPlainTextEntry(archive, "1 Exception", stringBuilder.ToString());
	}

	private static void AddPlayerLogEntry(ZipArchive archive)
	{
		string text = Path.Combine(Application.temporaryCachePath, "TimberbornTemporaryLog");
		File.Copy(Application.consoleLogPath, text, overwrite: true);
		string text2 = File.ReadAllText(text);
		AddPlainTextEntry(archive, "2 Player log", text2);
	}

	private static void AddStartingItemEntry(ZipArchive archive)
	{
		ImmutableArray<byte> data = WorldDataService.Data;
		if (data != null && data.Length > 0)
		{
			string extension = Path.GetExtension(WorldDataService.SourceFileName);
			using Stream stream = archive.CreateEntry("5 Starting item " + reportTimestamp + extension).Open();
			stream.Write(data.AsSpan());
		}
	}

	private static void AddSaveEntry(ZipArchive archive)
	{
		byte[] exceptionSave = ExceptionSave;
		if (exceptionSave != null && exceptionSave.Length != 0)
		{
			using (Stream stream = archive.CreateEntry("6 Error save " + reportTimestamp + ".timber").Open())
			{
				stream.Write(ExceptionSave, 0, ExceptionSave.Length);
			}
		}
	}

	private static void AddPlainTextEntryToExistingReport(string name, string text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return;
		}
		Debug.Log("Adding " + name + " to error report: " + ReportFilePath);
		try
		{
			using FileStream stream = new FileStream(ReportFilePath, FileMode.Open);
			using ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Update);
			AddPlainTextEntry(archive, name, text);
		}
		catch (Exception arg)
		{
			Debug.LogWarning($"Failed to add {name} to error report: {arg}");
		}
	}

	private static void AddPlainTextEntry(ZipArchive archive, string name, string text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return;
		}
		using Stream stream = archive.CreateEntry(name + " " + reportTimestamp + ".txt").Open();
		using StreamWriter streamWriter = new StreamWriter(stream);
		streamWriter.WriteLine(ErrorReportSanitizer.Sanitize(text));
	}
}
