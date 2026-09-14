using Timberborn.MapRepositorySystem;
using Timberborn.SingletonSystem;
using Timberborn.SteamWorkshop;

namespace Timberborn.SteamWorkshopMapDownloading;

internal class SteamMapRepositoryChangeNotifier : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly MapRepository _mapRepository;

	public SteamMapRepositoryChangeNotifier(EventBus eventBus, MapRepository mapRepository)
	{
		_eventBus = eventBus;
		_mapRepository = mapRepository;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnItemInstalled(ItemInstalledEvent itemInstalledEvent)
	{
		_mapRepository.NotifyMapRepositoryChanged();
	}
}
