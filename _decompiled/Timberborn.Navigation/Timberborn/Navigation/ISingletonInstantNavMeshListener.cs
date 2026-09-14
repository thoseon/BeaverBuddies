using Timberborn.SingletonSystem;

namespace Timberborn.Navigation;

[Singleton]
public interface ISingletonInstantNavMeshListener
{
	void OnInstantNavMeshUpdated(NavMeshUpdate navMeshUpdate);
}
