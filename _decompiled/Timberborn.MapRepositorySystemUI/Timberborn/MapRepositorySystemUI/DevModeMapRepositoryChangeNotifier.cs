using Timberborn.Debugging;
using Timberborn.MapRepositorySystem;
using Timberborn.SingletonSystem;

namespace Timberborn.MapRepositorySystemUI;

internal class DevModeMapRepositoryChangeNotifier : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly MapRepository _mapRepository;

	public DevModeMapRepositoryChangeNotifier(EventBus eventBus, MapRepository mapRepository)
	{
		_eventBus = eventBus;
		_mapRepository = mapRepository;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnDevModeToggled(DevModeToggledEvent devModeToggledEvent)
	{
		_mapRepository.NotifyMapRepositoryChanged();
	}
}
