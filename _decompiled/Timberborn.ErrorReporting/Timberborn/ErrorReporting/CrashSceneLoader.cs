using UnityEngine;
using UnityEngine.SceneManagement;

namespace Timberborn.ErrorReporting;

public static class CrashSceneLoader
{
	private static readonly int CrashSceneIndex = 3;

	public static bool DevModeEnabled { get; set; }

	public static bool Enabled => FullCrashScreen;

	private static bool FullCrashScreen => true;

	internal static void LoadCrashSceneIfEnabled()
	{
		if (Enabled)
		{
			LoadCrashScene();
		}
	}

	private static void LoadCrashScene()
	{
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		SceneManager.LoadSceneAsync(CrashSceneIndex);
	}
}
