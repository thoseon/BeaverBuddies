using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.PopulationStatisticsSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.DwellingSystem;

public class GlobalDwellingStatisticsProvider : ILoadableSingleton, IDwellingStatisticsProvider
{
	private readonly EventBus _eventBus;

	private readonly List<DistrictDwellingStatisticsProvider> _districtDwellingStatisticsProviders = new List<DistrictDwellingStatisticsProvider>();

	public GlobalDwellingStatisticsProvider(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public DwellingStatistics GetDwellingStatistics()
	{
		DwellingStatistics result = new DwellingStatistics(0, 0);
		foreach (DistrictDwellingStatisticsProvider districtDwellingStatisticsProvider in _districtDwellingStatisticsProviders)
		{
			result += districtDwellingStatisticsProvider.GetDwellingStatistics();
		}
		return result;
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		DistrictDwellingStatisticsProvider component = enteredFinishedStateEvent.BlockObject.GetComponent<DistrictDwellingStatisticsProvider>();
		if (component != null)
		{
			_districtDwellingStatisticsProviders.Add(component);
		}
	}

	[OnEvent]
	public void OnExitedFinishedState(ExitedFinishedStateEvent exitedFinishedStateEvent)
	{
		DistrictDwellingStatisticsProvider component = exitedFinishedStateEvent.BlockObject.GetComponent<DistrictDwellingStatisticsProvider>();
		if (component != null)
		{
			_districtDwellingStatisticsProviders.Remove(component);
		}
	}
}
