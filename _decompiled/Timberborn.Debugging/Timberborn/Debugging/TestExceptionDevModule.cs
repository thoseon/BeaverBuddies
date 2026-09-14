using System;
using Timberborn.Versioning;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace Timberborn.Debugging;

public class TestExceptionDevModule : IDevModule
{
	private readonly DevModeManager _devModeManager;

	public TestExceptionDevModule(DevModeManager devModeManager)
	{
		_devModeManager = devModeManager;
	}

	public DevModuleDefinition GetDefinition()
	{
		DevModuleDefinition.Builder builder = new DevModuleDefinition.Builder();
		if (GameVersions.CurrentVersion.IsDevelopmentVersion)
		{
			builder.AddMethod(DevMethod.Create("Test exception", ThrowTestException));
			builder.AddMethod(DevMethod.Create("Test exception non-dev", ThrowTestExceptionNonDev));
			builder.AddMethod(DevMethod.Create("Test native abort", Abort));
			builder.AddMethod(DevMethod.Create("Test warning", Warn));
		}
		return builder.Build();
	}

	private static void ThrowTestException()
	{
		throw new Exception("Test");
	}

	private void ThrowTestExceptionNonDev()
	{
		_devModeManager.Disable();
		ThrowTestException();
	}

	private static void Abort()
	{
		Utils.ForceCrash(ForcedCrashCategory.Abort);
	}

	private static void Warn()
	{
		Debug.LogWarning("Test warning");
	}
}
