using Timberborn.Autosaving;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;

namespace Timberborn.GameStartup;

internal class GameStartupAutosaveBlocker : IAutosaveBlocker, ILoadableSingleton
{
	private readonly EventBus _eventBus;

	public bool IsBlocking { get; private set; } = true;

	public GameStartupAutosaveBlocker(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		IsBlocking = false;
		_eventBus.Unregister(this);
	}
}
