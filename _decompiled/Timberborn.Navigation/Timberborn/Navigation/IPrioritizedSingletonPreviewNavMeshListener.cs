using Timberborn.SingletonSystem;

namespace Timberborn.Navigation;

[Singleton]
internal interface IPrioritizedSingletonPreviewNavMeshListener
{
	void OnPreviewNavMeshUpdated(NavMeshUpdate navMeshUpdate);
}
