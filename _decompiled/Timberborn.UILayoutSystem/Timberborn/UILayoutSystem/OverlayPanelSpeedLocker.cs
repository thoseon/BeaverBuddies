using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;

namespace Timberborn.UILayoutSystem;

public class OverlayPanelSpeedLocker : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly SpeedManager _speedManager;

	public OverlayPanelSpeedLocker(EventBus eventBus, SpeedManager speedManager)
	{
		_eventBus = eventBus;
		_speedManager = speedManager;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnPanelShown(PanelShownEvent panelShownEvent)
	{
		if (panelShownEvent.LockSpeed)
		{
			_speedManager.ChangeAndLockSpeed(0f);
		}
	}

	[OnEvent]
	public void OnPanelHidden(PanelHiddenEvent panelHiddenEvent)
	{
		if (panelHiddenEvent.UnlockSpeed)
		{
			_speedManager.UnlockSpeed();
		}
	}
}
