using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;

namespace Timberborn.MapEditorStartup;

public class MapEditorInitializer : IUpdatableSingleton
{
	private bool _alreadyInitialized;

	private readonly EventBus _eventBus;

	public MapEditorInitializer(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void UpdateSingleton()
	{
		if (!_alreadyInitialized)
		{
			_eventBus.Post(new ShowPrimaryUIEvent());
			_alreadyInitialized = true;
		}
	}
}
