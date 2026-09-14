using Timberborn.SingletonSystem;

namespace Timberborn.Navigation;

[Singleton]
internal interface IPrioritizedSingletonInstantNavMeshListener
{
	void OnInstantNavMeshUpdated(NavMeshUpdate navMeshUpdate);
}
