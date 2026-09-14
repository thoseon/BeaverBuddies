using System;
using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.Navigation;
using Timberborn.SingletonSystem;

namespace Timberborn.GameDistricts;

public class DistrictConnections : ISingletonInstantNavMeshListener, ILoadableSingleton
{
	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private readonly EventBus _eventBus;

	private readonly List<DistrictCluster> _districtClusters = new List<DistrictCluster>();

	private ReadOnlyList<DistrictCenter> DistrictCenters => _districtCenterRegistry.FinishedDistrictCenters;

	public DistrictConnections(DistrictCenterRegistry districtCenterRegistry, EventBus eventBus)
	{
		_districtCenterRegistry = districtCenterRegistry;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public void OnInstantNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		ReassignDistricts();
	}

	[OnEvent]
	public void OnDistrictCenterRegistryChanged(DistrictCenterRegistryChangedEvent districtCenterRegistryChangedEvent)
	{
		ReassignDistricts();
	}

	public IEnumerable<DistrictCenter> GetDistrictsConnectedWith(DistrictCenter districtCenter)
	{
		return GetDistrictCluster(districtCenter).GetDistrictsOtherThan(districtCenter);
	}

	public bool AreDistrictsConnected(DistrictCenter firstDistrict, DistrictCenter secondDistrict)
	{
		return GetDistrictCluster(firstDistrict).Contains(secondDistrict);
	}

	public DistrictCenter GetFirstConnectedOrAny(DistrictCenter districtCenter)
	{
		using (IEnumerator<DistrictCenter> enumerator = GetDistrictsConnectedWith(districtCenter).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		for (int i = 0; i < DistrictCenters.Count; i++)
		{
			if (DistrictCenters[i] != districtCenter)
			{
				return DistrictCenters[i];
			}
		}
		return districtCenter;
	}

	private DistrictCluster GetDistrictCluster(DistrictCenter districtCenter)
	{
		for (int i = 0; i < _districtClusters.Count; i++)
		{
			if (_districtClusters[i].Contains(districtCenter))
			{
				return _districtClusters[i];
			}
		}
		throw new NotSupportedException("Found DistrictCenter: " + districtCenter.DistrictName + " not assigned to any DistrictCluster");
	}

	private void ReassignDistricts()
	{
		_districtClusters.Clear();
		for (int i = 0; i < _districtCenterRegistry.FinishedDistrictCenters.Count; i++)
		{
			AddDistrictToCluster(_districtCenterRegistry.FinishedDistrictCenters[i]);
		}
		_eventBus.Post(new DistrictConnectionsChangedEvent());
	}

	private void AddDistrictToCluster(DistrictCenter districtCenter)
	{
		if (!TryAddDistrictToExistingCluster(districtCenter))
		{
			AddNewClusterWithDistrict(districtCenter);
		}
	}

	private bool TryAddDistrictToExistingCluster(DistrictCenter districtCenter)
	{
		for (int i = 0; i < _districtClusters.Count; i++)
		{
			if (_districtClusters[i].TryAddDistrict(districtCenter))
			{
				return true;
			}
		}
		return false;
	}

	private void AddNewClusterWithDistrict(DistrictCenter districtCenter)
	{
		DistrictCluster item = new DistrictCluster(districtCenter);
		_districtClusters.Add(item);
	}
}
