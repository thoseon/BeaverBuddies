using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Timberborn.PlatformUtilities;

public static class ProblematicProcessorInfo
{
	private static readonly ImmutableArray<string> ProblematicProcessors = ((IEnumerable<string>)new string[20]
	{
		"14900K", "14900KF", "14900KS", "14900F", "13900K", "13900KF", "13900KS", "13900F", "14700K", "14700KF",
		"14700F", "14700", "13700K", "13700KF", "13700F", "14790F", "14600K", "14600F", "13600K", "13600KF"
	}).ToImmutableArray();

	private static readonly uint HKEY_LOCAL_MACHINE = 2147483650u;

	private static readonly int KEY_READ = 131097;

	private static bool? isProblematicProcessor;

	private static string microcodeVersion;

	public static bool IsProblematic()
	{
		bool valueOrDefault = isProblematicProcessor == true;
		if (!isProblematicProcessor.HasValue)
		{
			valueOrDefault = IsProblematicUncached();
			isProblematicProcessor = valueOrDefault;
			return valueOrDefault;
		}
		return valueOrDefault;
	}

	public static string GetMicrocodeVersion()
	{
		return microcodeVersion ?? (microcodeVersion = GetMicrocodeVersionUncached());
	}

	private static bool IsProblematicUncached()
	{
		try
		{
			string processorType = SystemInfo.processorType;
			return ProblematicProcessors.Any((string problematicProcessor) => Regex.IsMatch(processorType, problematicProcessor + "(?![A-Za-z])", RegexOptions.IgnoreCase));
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			return false;
		}
	}

	private static string GetMicrocodeVersionUncached()
	{
		try
		{
			return (ApplicationPlatform.IsWindows() && IsProblematic()) ? ReadRegistryKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0", "Update Revision") : "";
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			return "";
		}
	}

	private static string ReadRegistryKey(string subKey, string valueName)
	{
		if (RegOpenKeyEx((UIntPtr)HKEY_LOCAL_MACHINE, subKey, 0, KEY_READ, out var phkResult) == 0)
		{
			uint lpcbData = 1024u;
			byte[] array = new byte[lpcbData];
			if (RegQueryValueEx(phkResult, valueName, 0, out var lpType, array, ref lpcbData) == 0)
			{
				RegCloseKey(phkResult);
				switch (lpType)
				{
				case 1u:
					return Encoding.Unicode.GetString(array, 0, (int)(lpcbData - 2));
				case 3u:
				case 4u:
					return $"0x{BitConverter.ToUInt32(array, 0):X8}";
				}
			}
			RegCloseKey(phkResult);
		}
		return "";
	}

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
	private static extern int RegOpenKeyEx(UIntPtr hKey, string subKey, int ulOptions, int samDesired, out IntPtr phkResult);

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
	private static extern int RegQueryValueEx(IntPtr hKey, string lpValueName, int lpReserved, out uint lpType, byte[] lpData, ref uint lpcbData);

	[DllImport("advapi32.dll")]
	private static extern int RegCloseKey(IntPtr hKey);
}
