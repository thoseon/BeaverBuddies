namespace Timberborn.Navigation;

internal class NavMeshUpdateNotifier
{
	private readonly NavMeshListenerSingletonRegistry _navMeshListenerSingletonRegistry;

	private readonly INavMeshListenerEntityRegistry _navMeshListenerEntityRegistry;

	public NavMeshUpdateNotifier(NavMeshListenerSingletonRegistry navMeshListenerSingletonRegistry, INavMeshListenerEntityRegistry navMeshListenerEntityRegistry)
	{
		_navMeshListenerSingletonRegistry = navMeshListenerSingletonRegistry;
		_navMeshListenerEntityRegistry = navMeshListenerEntityRegistry;
	}

	public void NotifyOfNavMeshUpdates(NavMeshUpdate navMeshUpdate)
	{
		_navMeshListenerSingletonRegistry.NotifyAll(navMeshUpdate);
		_navMeshListenerEntityRegistry.NotifyAll(navMeshUpdate);
	}

	public void NotifyOfPreviewNavMeshUpdates(NavMeshUpdate navMeshUpdate)
	{
		_navMeshListenerSingletonRegistry.NotifyAllPreview(navMeshUpdate);
	}

	public void NotifyOfInstantNavMeshUpdates(NavMeshUpdate navMeshUpdate)
	{
		_navMeshListenerSingletonRegistry.NotifyAllInstant(navMeshUpdate);
		_navMeshListenerEntityRegistry.NotifyAllInstant(navMeshUpdate);
	}
}
