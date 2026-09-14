using Timberborn.Autosaving;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;

namespace Timberborn.AutosavingUI;

internal class PanelAutosaveBlocker : ILoadableSingleton, IAutosaveBlocker
{
	private readonly EventBus _eventBus;

	public bool IsBlocking { get; private set; }

	public PanelAutosaveBlocker(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnPanelShown(PanelShownEvent panelShownEvent)
	{
		IsBlocking |= panelShownEvent.LockSpeed;
	}

	[OnEvent]
	public void OnPanelHidden(PanelHiddenEvent panelHiddenEvent)
	{
		IsBlocking = !panelHiddenEvent.UnlockSpeed;
	}
}
