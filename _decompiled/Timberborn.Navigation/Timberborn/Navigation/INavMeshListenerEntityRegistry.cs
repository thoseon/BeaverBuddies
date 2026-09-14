namespace Timberborn.Navigation;

public interface INavMeshListenerEntityRegistry
{
	void NotifyAll(NavMeshUpdate navMeshUpdate);

	void NotifyAllInstant(NavMeshUpdate navMeshUpdate);

	void RegisterNavMeshListener(INavMeshListener navMeshListener);

	void UnregisterNavMeshListener(INavMeshListener navMeshListener);

	void RegisterInstantNavMeshListener(IInstantNavMeshListener instantNavMeshListener);

	void UnregisterInstantNavMeshListener(IInstantNavMeshListener instantNavMeshListener);
}
