using Timberborn.GameDistricts;
using Timberborn.Population;
using Timberborn.SingletonSystem;

namespace Timberborn.PopulationUI;

internal class PopulationServiceDistrictSwitcher : ILoadableSingleton
{
	private readonly PopulationService _populationService;

	private readonly DistrictContextService _districtContextService;

	private readonly EventBus _eventBus;

	public PopulationServiceDistrictSwitcher(PopulationService populationService, DistrictContextService districtContextService, EventBus eventBus)
	{
		_populationService = populationService;
		_districtContextService = districtContextService;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnDistrictSelected(DistrictSelectedEvent districtSelectedEvent)
	{
		UpdateDistrict();
	}

	[OnEvent]
	public void OnDistrictUnselected(DistrictUnselectedEvent districtUnselectedEvent)
	{
		UpdateDistrict();
	}

	private void UpdateDistrict()
	{
		_populationService.SwitchDistrict(_districtContextService.SelectedDistrict);
	}
}
