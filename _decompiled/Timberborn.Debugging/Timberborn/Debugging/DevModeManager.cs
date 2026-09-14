using Timberborn.ErrorReporting;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Debugging;

public class DevModeManager
{
	private readonly EventBus _eventBus;

	public bool Enabled { get; private set; }

	public DevModeManager(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Enable()
	{
		if (!Enabled)
		{
			Debug.Log("Dev mode enabled");
			EnableSilently();
		}
	}

	public void Disable()
	{
		if (Enabled)
		{
			Enabled = false;
			CrashSceneLoader.DevModeEnabled = false;
			_eventBus.Post(new DevModeToggledEvent(enabled: false));
		}
	}

	private void EnableSilently()
	{
		if (!Enabled)
		{
			Enabled = true;
			CrashSceneLoader.DevModeEnabled = true;
			_eventBus.Post(new DevModeToggledEvent(enabled: true));
		}
	}
}
