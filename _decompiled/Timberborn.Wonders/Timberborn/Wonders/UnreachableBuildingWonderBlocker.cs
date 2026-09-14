using Timberborn.BaseComponentSystem;
using Timberborn.GameDistricts;

namespace Timberborn.Wonders;

public class UnreachableBuildingWonderBlocker : BaseComponent, IAwakableComponent, IWonderBlocker
{
	private DistrictBuilding _districtBuilding;

	public void Awake()
	{
		_districtBuilding = GetComponent<DistrictBuilding>();
	}

	public bool IsWonderBlocked()
	{
		return !_districtBuilding.InstantDistrict;
	}
}
