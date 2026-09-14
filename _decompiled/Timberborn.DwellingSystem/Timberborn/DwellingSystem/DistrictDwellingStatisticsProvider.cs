using Timberborn.BaseComponentSystem;
using Timberborn.GameDistricts;
using Timberborn.PopulationStatisticsSystem;

namespace Timberborn.DwellingSystem;

public class DistrictDwellingStatisticsProvider : BaseComponent, IAwakableComponent, IDwellingStatisticsProvider
{
	private DwellingStatistics _districtDwellingStatistics = new DwellingStatistics(0, 0);

	public void Awake()
	{
		DistrictBuildingRegistry component = GetComponent<DistrictBuildingRegistry>();
		component.FinishedBuildingRegistered += OnFinishedBuildingRegistered;
		component.FinishedBuildingUnregistered += OnFinishedBuildingUnregistered;
	}

	public DwellingStatistics GetDwellingStatistics()
	{
		return _districtDwellingStatistics;
	}

	private void OnFinishedBuildingRegistered(object sender, FinishedBuildingRegisteredEventArgs finishedBuildingRegisteredEventArgs)
	{
		DwellerCounter component = finishedBuildingRegisteredEventArgs.Building.GetComponent<DwellerCounter>();
		if (component != null)
		{
			AddDwellingBedCounter(component);
		}
	}

	private void OnFinishedBuildingUnregistered(object sender, FinishedBuildingUnregisteredEventArgs finishedBuildingUnregisteredEventArgs)
	{
		DwellerCounter component = finishedBuildingUnregisteredEventArgs.Building.GetComponent<DwellerCounter>();
		if (component != null)
		{
			RemoveDwellingBedCounter(component);
		}
	}

	private void AddDwellingBedCounter(DwellerCounter dwellerCounter)
	{
		_districtDwellingStatistics += dwellerCounter.GetCurrentDwellingStatistics();
		dwellerCounter.DwellerCountChanged += OnDwellerCountChanged;
	}

	private void RemoveDwellingBedCounter(DwellerCounter dwellerCounter)
	{
		_districtDwellingStatistics -= dwellerCounter.GetCurrentDwellingStatistics();
		dwellerCounter.DwellerCountChanged -= OnDwellerCountChanged;
	}

	private void OnDwellerCountChanged(object sender, DwellingChangedEventArgs dwellingChangedEventArgs)
	{
		_districtDwellingStatistics -= dwellingChangedEventArgs.OldDwellingStatistics;
		_districtDwellingStatistics += dwellingChangedEventArgs.NewDwellingStatistics;
	}
}
