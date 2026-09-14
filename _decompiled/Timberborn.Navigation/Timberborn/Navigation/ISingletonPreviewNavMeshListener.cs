using Timberborn.SingletonSystem;

namespace Timberborn.Navigation;

[Singleton]
public interface ISingletonPreviewNavMeshListener
{
	void OnPreviewNavMeshUpdated(NavMeshUpdate navMeshUpdate);
}
