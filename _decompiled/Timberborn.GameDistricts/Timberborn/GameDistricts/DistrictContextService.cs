using Timberborn.SingletonSystem;

namespace Timberborn.GameDistricts;

public class DistrictContextService
{
	private readonly EventBus _eventBus;

	public DistrictCenter SelectedDistrict { get; private set; }

	public DistrictContextService(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void SelectDistrict(DistrictCenter districtCenter)
	{
		if (SelectedDistrict != districtCenter)
		{
			UnselectDistrict();
			SelectedDistrict = districtCenter;
			_eventBus.Post(new DistrictSelectedEvent(districtCenter));
		}
	}

	public void UnselectDistrict()
	{
		if ((bool)SelectedDistrict)
		{
			SelectedDistrict = null;
			_eventBus.Post(new DistrictUnselectedEvent());
		}
	}
}
