using JetBrains.Annotations;
using Steamworks;
using Timberborn.SingletonSystem;
using Timberborn.SteamStoreSystem;

namespace Timberborn.SteamWorkshop;

internal class ItemInstalledNotifier : ILoadableSingleton, IUnloadableSingleton
{
	private readonly EventBus _eventBus;

	[UsedImplicitly]
	private Callback<ItemInstalled_t> _installationCallback;

	public ItemInstalledNotifier(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_installationCallback = Callback<ItemInstalled_t>.Create(OnItemInstalled);
	}

	public void Unload()
	{
		_installationCallback?.Dispose();
		_installationCallback = null;
	}

	private void OnItemInstalled(ItemInstalled_t itemInstalled)
	{
		if (itemInstalled.m_unAppID == SteamAppId.AppId)
		{
			_eventBus.Post(new ItemInstalledEvent());
		}
	}
}
