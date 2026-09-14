using System;
using System.IO;
using System.Net.Http;
using Timberborn.CommandLine;
using UnityEngine;

namespace Timberborn.ErrorReporting;

public static class ErrorReportSender
{
	private static readonly string Url = "https://api.timberborn.com/v1/upload-error-report";

	private static readonly string CustomUrlKey = "errorUrl";

	public static bool SendErrorReport(string comment, string email)
	{
		try
		{
			ErrorReporter.AddCommentToReport(comment);
			ErrorReporter.AddEmailToReport(email);
			UploadErrorReport();
		}
		catch (Exception ex)
		{
			Debug.Log("Error when submitting a crash report:\n" + ex);
			return false;
		}
		return true;
	}

	private static void UploadErrorReport()
	{
		using HttpClient httpClient = new HttpClient();
		using FileStream content = new FileStream(ErrorReporter.ReportFilePath, FileMode.Open);
		using StreamContent content2 = new StreamContent(content);
		httpClient.PostAsync(GetUrl(), content2).Result.EnsureSuccessStatusCode();
	}

	private static string GetUrl()
	{
		try
		{
			CommandLineArguments commandLineArguments = CommandLineArguments.CreateWithCommandLineArgs();
			if (commandLineArguments.Has(CustomUrlKey))
			{
				return commandLineArguments.GetString(CustomUrlKey);
			}
		}
		catch
		{
		}
		return Url;
	}
}
