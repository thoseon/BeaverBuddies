using Timberborn.SingletonSystem;
using Timberborn.TickSystem;

namespace Timberborn.Navigation;

internal class NavigationSynchronizer : ITickableSingleton, ILoadableSingleton, ILateUpdatableSingleton, IPostLoadableSingleton, INavigationPhase
{
	private readonly NavMeshUpdater _navMeshUpdater;

	private readonly NavMeshUpdateNotifier _navMeshUpdateNotifier;

	private readonly DistrictUpdater _districtUpdater;

	private readonly NavMeshUpdateBuilderFactory _navMeshUpdateBuilderFactory;

	private readonly RestrictedNodeUpdater _restrictedNodeUpdater;

	private NavMeshUpdate.Builder _regularNavMeshUpdateBuilder;

	private NavMeshUpdate.Builder _previewNavMeshUpdateBuilder;

	private NavMeshUpdate.Builder _instantNavMeshUpdateBuilder;

	public NavigationSynchronizer(NavMeshUpdater navMeshUpdater, NavMeshUpdateNotifier navMeshUpdateNotifier, DistrictUpdater districtUpdater, NavMeshUpdateBuilderFactory navMeshUpdateBuilderFactory, RestrictedNodeUpdater restrictedNodeUpdater)
	{
		_navMeshUpdater = navMeshUpdater;
		_navMeshUpdateNotifier = navMeshUpdateNotifier;
		_districtUpdater = districtUpdater;
		_navMeshUpdateBuilderFactory = navMeshUpdateBuilderFactory;
		_restrictedNodeUpdater = restrictedNodeUpdater;
	}

	public void Load()
	{
		_regularNavMeshUpdateBuilder = _navMeshUpdateBuilderFactory.Create();
		_previewNavMeshUpdateBuilder = _navMeshUpdateBuilderFactory.Create();
		_instantNavMeshUpdateBuilder = _navMeshUpdateBuilderFactory.Create();
	}

	public void PostLoad()
	{
		ProcessPreviewChanges();
		ProcessInstantChanges();
		ProcessRegularChanges();
		_navMeshUpdater.TrimExcess();
		NotifyAllNavmeshChanges();
	}

	public void Tick()
	{
		ProcessRegularChanges();
		NotifyAllNavmeshChanges();
	}

	public void LateUpdateSingleton()
	{
		ProcessPreviewChanges();
		ProcessInstantChanges();
		NotifyAllNavmeshChanges();
	}

	private void ProcessRegularChanges()
	{
		_navMeshUpdater.ProcessRegularChanges(_regularNavMeshUpdateBuilder);
		_districtUpdater.ProcessRegularChanges(_regularNavMeshUpdateBuilder);
		_restrictedNodeUpdater.ProcessRegularChanges();
	}

	private void ProcessPreviewChanges()
	{
		_navMeshUpdater.ProcessPreviewChanges(_previewNavMeshUpdateBuilder);
	}

	private void ProcessInstantChanges()
	{
		_navMeshUpdater.ProcessInstantChanges(_instantNavMeshUpdateBuilder);
		_districtUpdater.ProcessInstantChanges(_instantNavMeshUpdateBuilder);
	}

	private void NotifyAllNavmeshChanges()
	{
		if (!_regularNavMeshUpdateBuilder.IsEmpty)
		{
			NavMeshUpdate navMeshUpdate = _regularNavMeshUpdateBuilder.Build();
			_navMeshUpdateNotifier.NotifyOfNavMeshUpdates(navMeshUpdate);
			_regularNavMeshUpdateBuilder.Reset();
		}
		if (!_previewNavMeshUpdateBuilder.IsEmpty)
		{
			NavMeshUpdate navMeshUpdate2 = _previewNavMeshUpdateBuilder.Build();
			_navMeshUpdateNotifier.NotifyOfPreviewNavMeshUpdates(navMeshUpdate2);
			_previewNavMeshUpdateBuilder.Reset();
		}
		if (!_instantNavMeshUpdateBuilder.IsEmpty)
		{
			NavMeshUpdate navMeshUpdate3 = _instantNavMeshUpdateBuilder.Build();
			_navMeshUpdateNotifier.NotifyOfInstantNavMeshUpdates(navMeshUpdate3);
			_navMeshUpdateNotifier.NotifyOfPreviewNavMeshUpdates(navMeshUpdate3);
			_instantNavMeshUpdateBuilder.Reset();
		}
	}
}
