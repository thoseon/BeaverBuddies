using System.Collections.Immutable;

namespace Timberborn.SingletonSystem;

internal class SingletonLifecycleService
{
	private readonly ISingletonRepository _singletonRepository;

	private bool _initializedSuccessfully;

	private ImmutableArray<ILoadableSingleton> _loadableSingletons;

	private ImmutableArray<INonSingletonLoader> _nonSingletonLoaders;

	private ImmutableArray<IPostLoadableSingleton> _postLoadableSingletons;

	private ImmutableArray<INonSingletonPostLoader> _nonSingletonPostLoaders;

	private ImmutableArray<IUnloadableSingleton> _unloadableSingletons;

	private ImmutableArray<IUpdatableSingleton> _updatableSingletons;

	private ImmutableArray<ILateUpdatableSingleton> _lateUpdatableSingletons;

	public SingletonLifecycleService(ISingletonRepository singletonRepository)
	{
		_singletonRepository = singletonRepository;
	}

	public void LoadAll()
	{
		_loadableSingletons = _singletonRepository.GetSingletons<ILoadableSingleton>().ToImmutableArray();
		_nonSingletonLoaders = _singletonRepository.GetSingletons<INonSingletonLoader>().ToImmutableArray();
		_postLoadableSingletons = _singletonRepository.GetSingletons<IPostLoadableSingleton>().ToImmutableArray();
		_nonSingletonPostLoaders = _singletonRepository.GetSingletons<INonSingletonPostLoader>().ToImmutableArray();
		_unloadableSingletons = _singletonRepository.GetSingletons<IUnloadableSingleton>().ToImmutableArray();
		_updatableSingletons = _singletonRepository.GetSingletons<IUpdatableSingleton>().ToImmutableArray();
		_lateUpdatableSingletons = _singletonRepository.GetSingletons<ILateUpdatableSingleton>().ToImmutableArray();
		LoadSingletons();
		LoadNonSingletons();
		PostLoadSingletons();
		PostLoadNonSingletons();
		_initializedSuccessfully = true;
	}

	public void UnloadAll()
	{
		UnloadSingletons();
	}

	public void UpdateAll()
	{
		if (_initializedSuccessfully)
		{
			UpdateSingletons();
		}
	}

	public void LateUpdateAll()
	{
		if (_initializedSuccessfully)
		{
			LateUpdateSingletons();
		}
	}

	private void LoadSingletons()
	{
		for (int i = 0; i < _loadableSingletons.Length; i++)
		{
			_loadableSingletons[i].Load();
		}
	}

	private void LoadNonSingletons()
	{
		for (int i = 0; i < _nonSingletonLoaders.Length; i++)
		{
			_nonSingletonLoaders[i].LoadNonSingletons();
		}
	}

	private void PostLoadSingletons()
	{
		for (int i = 0; i < _postLoadableSingletons.Length; i++)
		{
			_postLoadableSingletons[i].PostLoad();
		}
	}

	private void PostLoadNonSingletons()
	{
		for (int i = 0; i < _nonSingletonPostLoaders.Length; i++)
		{
			_nonSingletonPostLoaders[i].PostLoadNonSingletons();
		}
	}

	private void UnloadSingletons()
	{
		for (int i = 0; i < _unloadableSingletons.Length; i++)
		{
			_unloadableSingletons[i].Unload();
		}
	}

	private void UpdateSingletons()
	{
		for (int i = 0; i < _updatableSingletons.Length; i++)
		{
			_updatableSingletons[i].UpdateSingleton();
		}
	}

	private void LateUpdateSingletons()
	{
		for (int i = 0; i < _lateUpdatableSingletons.Length; i++)
		{
			_lateUpdatableSingletons[i].LateUpdateSingleton();
		}
	}
}
