using System.Collections.Generic;
using Timberborn.LevelVisibilitySystem;
using Timberborn.Navigation;
using Timberborn.SingletonSystem;

namespace Timberborn.BuildingsNavigation;

internal class PathNavRangeDrawerInvalidator : ILoadableSingleton, ISingletonInstantNavMeshListener
{
	private readonly EventBus _eventBus;

	private readonly List<DistrictPathNavRangeDrawer> _districtPathNavRangeDrawers = new List<DistrictPathNavRangeDrawer>();

	public PathNavRangeDrawerInvalidator(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnMaxVisibleLevelChanged(MaxVisibleLevelChangedEvent maxVisibleLevelChangedEvent)
	{
		MarkDistrictDrawersDirty();
	}

	public void OnInstantNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		MarkDistrictDrawersDirty();
	}

	public void AddDistrictDrawer(DistrictPathNavRangeDrawer districtPathNavRangeDrawer)
	{
		_districtPathNavRangeDrawers.Add(districtPathNavRangeDrawer);
	}

	public void RemoveDistrictDrawer(DistrictPathNavRangeDrawer districtPathNavRangeDrawer)
	{
		_districtPathNavRangeDrawers.Remove(districtPathNavRangeDrawer);
	}

	private void MarkDistrictDrawersDirty()
	{
		foreach (DistrictPathNavRangeDrawer districtPathNavRangeDrawer in _districtPathNavRangeDrawers)
		{
			districtPathNavRangeDrawer.MarkDirty();
		}
	}
}
