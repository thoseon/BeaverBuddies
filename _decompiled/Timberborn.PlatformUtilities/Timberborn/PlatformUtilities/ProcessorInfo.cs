using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Timberborn.PlatformUtilities;

public static class ProcessorInfo
{
	public static bool IsAppleCpu()
	{
		return SystemInfo.processorType.ToLowerInvariant().Contains("apple");
	}

	public static bool IsIntelProcess()
	{
		return RuntimeInformation.ProcessArchitecture == Architecture.X64;
	}

	public static int GetPhysicalProcessorCount()
	{
		if (!IsAppleCpu())
		{
			return Environment.ProcessorCount / 2;
		}
		return Environment.ProcessorCount;
	}
}
