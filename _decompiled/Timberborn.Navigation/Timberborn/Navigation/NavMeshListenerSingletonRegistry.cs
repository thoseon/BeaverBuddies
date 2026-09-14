using System.Collections.Immutable;
using Timberborn.SingletonSystem;

namespace Timberborn.Navigation;

internal class NavMeshListenerSingletonRegistry : ILoadableSingleton
{
	private readonly ISingletonRepository _singletonRepository;

	private ImmutableArray<ISingletonNavMeshListener> _navMeshListeners;

	private ImmutableArray<IPrioritizedSingletonNavMeshListener> _prioritizedNavMeshListeners;

	private ImmutableArray<ISingletonPreviewNavMeshListener> _previewNavMeshListeners;

	private ImmutableArray<IPrioritizedSingletonPreviewNavMeshListener> _prioritizedPreviewNavMeshListeners;

	private ImmutableArray<ISingletonInstantNavMeshListener> _instantNavMeshListeners;

	private ImmutableArray<IPrioritizedSingletonInstantNavMeshListener> _prioritizedInstantNavMeshListeners;

	public NavMeshListenerSingletonRegistry(ISingletonRepository singletonRepository)
	{
		_singletonRepository = singletonRepository;
	}

	public void Load()
	{
		_navMeshListeners = _singletonRepository.GetSingletons<ISingletonNavMeshListener>().ToImmutableArray();
		_prioritizedNavMeshListeners = _singletonRepository.GetSingletons<IPrioritizedSingletonNavMeshListener>().ToImmutableArray();
		_previewNavMeshListeners = _singletonRepository.GetSingletons<ISingletonPreviewNavMeshListener>().ToImmutableArray();
		_prioritizedPreviewNavMeshListeners = _singletonRepository.GetSingletons<IPrioritizedSingletonPreviewNavMeshListener>().ToImmutableArray();
		_instantNavMeshListeners = _singletonRepository.GetSingletons<ISingletonInstantNavMeshListener>().ToImmutableArray();
		_prioritizedInstantNavMeshListeners = _singletonRepository.GetSingletons<IPrioritizedSingletonInstantNavMeshListener>().ToImmutableArray();
	}

	public void NotifyAll(NavMeshUpdate navMeshUpdate)
	{
		for (int i = 0; i < _prioritizedNavMeshListeners.Length; i++)
		{
			_prioritizedNavMeshListeners[i].OnNavMeshUpdated(navMeshUpdate);
		}
		for (int j = 0; j < _navMeshListeners.Length; j++)
		{
			_navMeshListeners[j].OnNavMeshUpdated(navMeshUpdate);
		}
	}

	public void NotifyAllPreview(NavMeshUpdate navMeshUpdate)
	{
		for (int i = 0; i < _prioritizedPreviewNavMeshListeners.Length; i++)
		{
			_prioritizedPreviewNavMeshListeners[i].OnPreviewNavMeshUpdated(navMeshUpdate);
		}
		for (int j = 0; j < _previewNavMeshListeners.Length; j++)
		{
			_previewNavMeshListeners[j].OnPreviewNavMeshUpdated(navMeshUpdate);
		}
	}

	public void NotifyAllInstant(NavMeshUpdate navMeshUpdate)
	{
		for (int i = 0; i < _prioritizedInstantNavMeshListeners.Length; i++)
		{
			_prioritizedInstantNavMeshListeners[i].OnInstantNavMeshUpdated(navMeshUpdate);
		}
		for (int j = 0; j < _instantNavMeshListeners.Length; j++)
		{
			_instantNavMeshListeners[j].OnInstantNavMeshUpdated(navMeshUpdate);
		}
	}
}
