using Timberborn.SingletonSystem;

namespace Timberborn.Navigation;

[Singleton]
public interface ISingletonNavMeshListener
{
	void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate);
}
