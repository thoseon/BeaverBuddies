using Timberborn.GameDistricts;
using Timberborn.ResourceCountingSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.ResourceCountingSystemUI;

public class ContextualResourceCountingService : ILoadableSingleton
{
	private readonly ResourceCountingService _resourceCountingService;

	private readonly DistrictContextService _districtContextService;

	private readonly EventBus _eventBus;

	public ContextualResourceCountingService(ResourceCountingService resourceCountingService, DistrictContextService districtContextService, EventBus eventBus)
	{
		_resourceCountingService = resourceCountingService;
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

	public ResourceCount GetContextualResourceCount(string goodId)
	{
		if (!_districtContextService.SelectedDistrict)
		{
			return _resourceCountingService.GetGlobalResourceCount(goodId);
		}
		return _resourceCountingService.GetDistrictResourceCount(goodId);
	}

	private void UpdateDistrict()
	{
		_resourceCountingService.SwitchDistrict(_districtContextService.SelectedDistrict);
	}
}
