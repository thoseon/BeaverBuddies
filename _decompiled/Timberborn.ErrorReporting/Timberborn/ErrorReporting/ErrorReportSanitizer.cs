using System.Text.RegularExpressions;

namespace Timberborn.ErrorReporting;

internal static class ErrorReportSanitizer
{
	private static readonly Regex WindowsAndMacRegex = new Regex("(?<prefix>[/\\\\]Users[/\\\\])[^/\\\\]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	private static readonly Regex LinuxRegex = new Regex("(?<prefix>/home/)[^/\\\\]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	public static string Sanitize(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return "";
		}
		string input2 = WindowsAndMacRegex.Replace(input, "${prefix}***");
		return LinuxRegex.Replace(input2, "${prefix}***");
	}
}
