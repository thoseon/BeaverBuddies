using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.GameDistricts;
using Timberborn.SingletonSystem;

namespace Timberborn.ResourceCountingSystem;

public class ResourceCountingService : ILoadableSingleton
{
	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private readonly EventBus _eventBus;

	private readonly Dictionary<DistrictCenter, DistrictResourceCounter> _counters = new Dictionary<DistrictCenter, DistrictResourceCounter>();

	private readonly DistrictResourceCounter _emptyDistrictResourceCounter = new DistrictResourceCounter();

	private DistrictResourceCounter _districtResourceCounter;

	public ResourceCountingService(DistrictCenterRegistry districtCenterRegistry, EventBus eventBus)
	{
		_districtCenterRegistry = districtCenterRegistry;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public void SwitchDistrict(DistrictCenter districtCenter)
	{
		_districtResourceCounter = GetResourceCounter(districtCenter);
	}

	public ResourceCount GetGlobalResourceCount(string goodId)
	{
		ResourceCount empty = ResourceCount.Empty;
		foreach (DistrictCenter finishedDistrictCenter in _districtCenterRegistry.FinishedDistrictCenters)
		{
			empty += GetDistrictResourceCounter(finishedDistrictCenter).GetResourceCount(goodId);
		}
		return empty;
	}

	public ResourceCount GetDistrictResourceCount(string goodId)
	{
		if (!_districtResourceCounter)
		{
			return ResourceCount.Empty;
		}
		return _districtResourceCounter.GetResourceCount(goodId);
	}

	public DistrictResourceCounter GetDistrictResourceCounter(DistrictCenter districtCenter)
	{
		return GetResourceCounter(districtCenter);
	}

	[OnEvent]
	public void OnExitedFinishedState(ExitedFinishedStateEvent exitedFinishedStateEvent)
	{
		if (exitedFinishedStateEvent.BlockObject.TryGetComponent<DistrictCenter>(out var component))
		{
			_counters.Remove(component);
		}
	}

	private DistrictResourceCounter GetResourceCounter(DistrictCenter districtCenter)
	{
		if (districtCenter == null)
		{
			return _emptyDistrictResourceCounter;
		}
		if (!_counters.TryGetValue(districtCenter, out var value))
		{
			value = districtCenter.GetComponent<DistrictResourceCounter>();
			if ((bool)value)
			{
				_counters.Add(districtCenter, value);
			}
		}
		return value;
	}
}
