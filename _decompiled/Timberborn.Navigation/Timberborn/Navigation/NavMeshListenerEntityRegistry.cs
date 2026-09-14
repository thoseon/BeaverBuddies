using System.Collections.Generic;

namespace Timberborn.Navigation;

internal class NavMeshListenerEntityRegistry : INavMeshListenerEntityRegistry
{
	private readonly List<INavMeshListener> _navMeshListeners = new List<INavMeshListener>();

	private readonly List<IInstantNavMeshListener> _instantNavMeshListeners = new List<IInstantNavMeshListener>();

	public void NotifyAll(NavMeshUpdate navMeshUpdate)
	{
		for (int i = 0; i < _navMeshListeners.Count; i++)
		{
			_navMeshListeners[i].OnNavMeshUpdated(navMeshUpdate);
		}
	}

	public void NotifyAllInstant(NavMeshUpdate navMeshUpdate)
	{
		for (int i = 0; i < _instantNavMeshListeners.Count; i++)
		{
			_instantNavMeshListeners[i].OnInstantNavMeshUpdated(navMeshUpdate);
		}
	}

	public void RegisterNavMeshListener(INavMeshListener navMeshListener)
	{
		_navMeshListeners.Add(navMeshListener);
	}

	public void UnregisterNavMeshListener(INavMeshListener navMeshListener)
	{
		_navMeshListeners.Remove(navMeshListener);
	}

	public void RegisterInstantNavMeshListener(IInstantNavMeshListener instantNavMeshListener)
	{
		_instantNavMeshListeners.Add(instantNavMeshListener);
	}

	public void UnregisterInstantNavMeshListener(IInstantNavMeshListener instantNavMeshListener)
	{
		_instantNavMeshListeners.Remove(instantNavMeshListener);
	}
}
