using Timberborn.GameDistricts;
using Timberborn.SingletonSystem;
using Timberborn.Wellbeing;

namespace Timberborn.WellbeingUI;

internal class WellbeingServiceDistrictSwitcher : ILoadableSingleton
{
	private readonly WellbeingService _wellbeingService;

	private readonly DistrictContextService _districtContextService;

	private readonly EventBus _eventBus;

	public WellbeingServiceDistrictSwitcher(WellbeingService wellbeingService, DistrictContextService districtContextService, EventBus eventBus)
	{
		_wellbeingService = wellbeingService;
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
		_wellbeingService.SwitchDistrict(_districtContextService.SelectedDistrict);
	}
}
