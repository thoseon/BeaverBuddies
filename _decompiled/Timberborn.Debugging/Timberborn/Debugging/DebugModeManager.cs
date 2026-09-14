using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Debugging;

public class DebugModeManager
{
	private readonly EventBus _eventBus;

	public bool Enabled { get; private set; }

	public DebugModeManager(EventBus eventBus)
	{
		_eventBus = eventBus;
		Enabled = false;
	}

	public void Enable()
	{
		if (!Enabled)
		{
			Debug.Log("Debug mode enabled");
			Enabled = true;
			_eventBus.Post(new DebugModeToggledEvent(enabled: true));
		}
	}

	public void Disable()
	{
		if (Enabled)
		{
			Enabled = false;
			_eventBus.Post(new DebugModeToggledEvent(enabled: false));
		}
	}
}
