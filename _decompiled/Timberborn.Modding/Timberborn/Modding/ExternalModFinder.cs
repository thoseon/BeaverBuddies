using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Timberborn.Modding;

public static class ExternalModFinder
{
	private static readonly List<string> Assemblies = new List<string> { "BepInEx" };

	private static readonly List<string> Processes = new List<string> { "WeMod", "Plitch", "TrainerManager" };

	public static void CheckForMods()
	{
		if (GetMods().Any())
		{
			ModdedState.SetUnofficialMods();
		}
	}

	private static IEnumerable<string> GetMods()
	{
		Assembly[] loadedAssemblies = GetAssemblies();
		foreach (string modAssembly in Assemblies)
		{
			if (loadedAssemblies.Any((Assembly loadedAssembly) => AssemblyIsMod(loadedAssembly, modAssembly)))
			{
				yield return modAssembly;
			}
		}
		Process[] activeProcesses = Process.GetProcesses();
		foreach (string modProcess in Processes)
		{
			if (activeProcesses.Any((Process activeProcess) => ProcessIsMod(activeProcess, modProcess)))
			{
				yield return modProcess;
			}
		}
	}

	private static Assembly[] GetAssemblies()
	{
		try
		{
			return AppDomain.CurrentDomain.GetAssemblies();
		}
		catch (AppDomainUnloadedException)
		{
			return Array.Empty<Assembly>();
		}
	}

	private static bool AssemblyIsMod(Assembly loadedAssembly, string modAssembly)
	{
		return loadedAssembly.FullName.Contains(modAssembly, StringComparison.OrdinalIgnoreCase);
	}

	private static bool ProcessIsMod(Process process, string modProcess)
	{
		try
		{
			return !process.HasExited && process.ProcessName.Contains(modProcess, StringComparison.OrdinalIgnoreCase);
		}
		catch (InvalidOperationException)
		{
			return false;
		}
		catch (NotSupportedException)
		{
			return false;
		}
	}
}
