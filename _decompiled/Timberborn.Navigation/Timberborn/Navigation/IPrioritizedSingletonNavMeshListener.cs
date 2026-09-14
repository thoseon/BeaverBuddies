using Timberborn.SingletonSystem;

namespace Timberborn.Navigation;

[Singleton]
internal interface IPrioritizedSingletonNavMeshListener
{
	void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate);
}
