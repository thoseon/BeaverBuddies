namespace Timberborn.Navigation;

public class DummyNavMeshListenerEntityRegistry : INavMeshListenerEntityRegistry
{
	public void NotifyAll(NavMeshUpdate navMeshUpdate)
	{
	}

	public void NotifyAllInstant(NavMeshUpdate navMeshUpdate)
	{
	}

	public void RegisterNavMeshListener(INavMeshListener navMeshListener)
	{
	}

	public void UnregisterNavMeshListener(INavMeshListener navMeshListener)
	{
	}

	public void RegisterInstantNavMeshListener(IInstantNavMeshListener instantNavMeshListener)
	{
	}

	public void UnregisterInstantNavMeshListener(IInstantNavMeshListener instantNavMeshListener)
	{
	}
}
