using System;
using System.IO;
using System.Text;
using AOT;
using Steamworks;
using Timberborn.FeatureToggleSystem;
using Timberborn.PlatformUtilities;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.SteamStoreSystem;

public class SteamManager : ILoadableSingleton, IUnloadableSingleton, IUpdatableSingleton
{
	private SteamAPIWarningMessageHook_t _steamAPIWarningMessageHook;

	public bool Initialized { get; private set; }

	public bool GameIsAllowedToRun { get; private set; }

	public void Load()
	{
		if (Application.isEditor && !FeatureToggles.SteamInEditor)
		{
			GameIsAllowedToRun = true;
			return;
		}
		TryToInitialize();
		if (Initialized && _steamAPIWarningMessageHook == null)
		{
			_steamAPIWarningMessageHook = SteamAPIDebugTextHook;
			SteamClient.SetWarningMessageHook(_steamAPIWarningMessageHook);
		}
	}

	public void Unload()
	{
		if (Initialized)
		{
			SteamAPI.Shutdown();
		}
	}

	public void UpdateSingleton()
	{
		if (Initialized)
		{
			SteamAPI.RunCallbacks();
		}
	}

	[MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
	internal static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
		Debug.LogWarning(pchDebugText);
	}

	private void TryToInitialize()
	{
		RunPacksizeTest();
		RunDllCheckTest();
		UseSteamInitializationWorkingDirectory(out var originalDirectory);
		if (!RestartAppIfNecessary())
		{
			GameIsAllowedToRun = true;
			if (InitializeSteamworksAPI())
			{
				Initialized = true;
				Debug.Log("Successfully connected to the Steam client.");
			}
			if (originalDirectory != null)
			{
				Directory.SetCurrentDirectory(originalDirectory);
			}
		}
	}

	private bool InitializeSteamworksAPI()
	{
		if (SteamAPI.Init())
		{
			return true;
		}
		if (Application.isEditor && FeatureToggles.SteamInEditor)
		{
			throw new InvalidOperationException("You are using SteamInEditor toggle, but the Steam couldn't be initialized. Make sure the Steam is running in the background");
		}
		Debug.Log("Couldn't connect to the Steam client. Is it running and do you have the game in your library?");
		return false;
	}

	private static void UseSteamInitializationWorkingDirectory(out string originalDirectory)
	{
		if (!ApplicationPlatform.IsMacOS())
		{
			originalDirectory = null;
			return;
		}
		originalDirectory = Directory.GetCurrentDirectory();
		DirectoryInfo parent = Directory.GetParent(originalDirectory);
		if (parent != null)
		{
			Directory.SetCurrentDirectory(parent.FullName);
			return;
		}
		throw new InvalidOperationException("Couldn't set the working directory to the parent of the current directory: " + originalDirectory);
	}

	private bool RestartAppIfNecessary()
	{
		try
		{
			if (SteamAPI.RestartAppIfNecessary(SteamAppId.AppId))
			{
				Application.Quit();
				return true;
			}
		}
		catch (DllNotFoundException ex)
		{
			Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + ex);
			Application.Quit();
			return true;
		}
		return false;
	}

	private void RunDllCheckTest()
	{
		if (!DllCheck.Test())
		{
			Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");
		}
	}

	private void RunPacksizeTest()
	{
		if (!Packsize.Test())
		{
			Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");
		}
	}
}
