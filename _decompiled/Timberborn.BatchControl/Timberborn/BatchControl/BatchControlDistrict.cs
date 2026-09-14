using Timberborn.GameDistricts;
using Timberborn.SingletonSystem;

namespace Timberborn.BatchControl;

public class BatchControlDistrict
{
	private readonly EventBus _eventBus;

	public DistrictCenter SelectedDistrict { get; private set; }

	public BatchControlDistrict(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void SetDistrict(DistrictCenter districtCenter)
	{
		if (districtCenter != SelectedDistrict)
		{
			SelectedDistrict = districtCenter;
			_eventBus.Post(new BatchControlDistrictChangedEvent());
		}
	}
}
