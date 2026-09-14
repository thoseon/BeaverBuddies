using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.SingletonSystem;

namespace Timberborn.GameDistricts;

public class DistrictCenterRegistry : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly List<DistrictCenter> _allDistrictCenters = new List<DistrictCenter>();

	private readonly List<DistrictCenter> _finishedDistrictCenters = new List<DistrictCenter>();

	public ReadOnlyList<DistrictCenter> AllDistrictCenters => _allDistrictCenters.AsReadOnlyList();

	public ReadOnlyList<DistrictCenter> FinishedDistrictCenters => _finishedDistrictCenters.AsReadOnlyList();

	public DistrictCenterRegistry(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnEnteredUnfinishedState(EnteredUnfinishedStateEvent enteredUnfinishedStateEvent)
	{
		if (enteredUnfinishedStateEvent.BlockObject.TryGetComponent<DistrictCenter>(out var component))
		{
			AddDistrictCenter(component);
		}
	}

	[OnEvent]
	public void OnExitedUnfinishedState(ExitedUnfinishedStateEvent exitedUnfinishedStateEvent)
	{
		if (exitedUnfinishedStateEvent.BlockObject.TryGetComponent<DistrictCenter>(out var component))
		{
			RemoveDistrictCenter(component);
		}
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		if (enteredFinishedStateEvent.BlockObject.TryGetComponent<DistrictCenter>(out var component))
		{
			RegisterFinishedDistrictCenter(component);
		}
	}

	[OnEvent]
	public void OnExitedFinishedState(ExitedFinishedStateEvent exitedFinishedStateEvent)
	{
		if (exitedFinishedStateEvent.BlockObject.TryGetComponent<DistrictCenter>(out var component))
		{
			UnregisterFinishedDistrictCenter(component);
		}
	}

	private void RegisterFinishedDistrictCenter(DistrictCenter districtCenter)
	{
		AddDistrictCenter(districtCenter);
		_finishedDistrictCenters.Add(districtCenter);
		_eventBus.Post(new DistrictCenterRegistryChangedEvent());
	}

	private void UnregisterFinishedDistrictCenter(DistrictCenter districtCenter)
	{
		RemoveDistrictCenter(districtCenter);
		_finishedDistrictCenters.Remove(districtCenter);
		_eventBus.Post(new DistrictCenterRegistryChangedEvent());
	}

	private void AddDistrictCenter(DistrictCenter districtCenter)
	{
		_allDistrictCenters.Add(districtCenter);
	}

	private void RemoveDistrictCenter(DistrictCenter districtCenter)
	{
		_allDistrictCenters.Remove(districtCenter);
	}
}
