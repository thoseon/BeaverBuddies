using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Timberborn.PlatformUtilities;

public class ExplorerOpener : IExplorerOpener
{
	public void OpenDirectory(string directory)
	{
		if (Directory.Exists(directory))
		{
			if (ApplicationPlatform.IsWindows())
			{
				StartProcessIgnoringExceptions("explorer.exe", directory.Replace("/", "\\"));
			}
			else if (ApplicationPlatform.IsMacOS())
			{
				StartProcessIgnoringExceptions("open", "\"" + directory.Replace("\\", "/") + "\"");
			}
		}
		else
		{
			UnityEngine.Debug.LogWarning("Directory " + directory + " does not exist.");
		}
	}

	private void StartProcessIgnoringExceptions(string fileName, string arguments)
	{
		try
		{
			Process.Start(fileName, arguments);
		}
		catch (Exception message)
		{
			UnityEngine.Debug.LogError(message);
		}
	}
}
