using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.PopulationStatisticsSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.PopulationWorkStatistics;

public class GlobalEmploymentStatisticsProvider : ILoadableSingleton, IEmploymentStatisticsProvider
{
	private readonly EventBus _eventBus;

	private readonly List<DistrictEmploymentStatisticsProvider> _districtEmploymentStatisticsProviders = new List<DistrictEmploymentStatisticsProvider>();

	public GlobalEmploymentStatisticsProvider(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public EmploymentStatistics GetEmploymentStatistics(string workerType)
	{
		EmploymentStatistics result = new EmploymentStatistics(0, 0, workerType);
		foreach (DistrictEmploymentStatisticsProvider districtEmploymentStatisticsProvider in _districtEmploymentStatisticsProviders)
		{
			result += districtEmploymentStatisticsProvider.GetEmploymentStatistics(workerType);
		}
		return result;
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		DistrictEmploymentStatisticsProvider component = enteredFinishedStateEvent.BlockObject.GetComponent<DistrictEmploymentStatisticsProvider>();
		if (component != null)
		{
			_districtEmploymentStatisticsProviders.Add(component);
		}
	}

	[OnEvent]
	public void OnExitedFinishedState(ExitedFinishedStateEvent exitedFinishedStateEvent)
	{
		DistrictEmploymentStatisticsProvider component = exitedFinishedStateEvent.BlockObject.GetComponent<DistrictEmploymentStatisticsProvider>();
		if (component != null)
		{
			_districtEmploymentStatisticsProviders.Remove(component);
		}
	}
}
