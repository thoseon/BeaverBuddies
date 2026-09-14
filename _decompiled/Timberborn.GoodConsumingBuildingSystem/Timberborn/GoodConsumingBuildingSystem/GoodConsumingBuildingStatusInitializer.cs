using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.GameDistricts;
using Timberborn.Hauling;
using Timberborn.InventorySystem;
using Timberborn.WorkSystem;
using Timberborn.Workshops;

namespace Timberborn.GoodConsumingBuildingSystem;

internal class GoodConsumingBuildingStatusInitializer : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private GoodConsumingBuilding _goodConsumingBuilding;

	private Workplace _workplace;

	private DistrictBuilding _districtBuilding;

	private LackOfResourcesStatus _lackOfResourcesStatus;

	private NoHaulingPostStatus _noHaulingPostStatus;

	public void Awake()
	{
		_goodConsumingBuilding = GetComponent<GoodConsumingBuilding>();
		_workplace = GetComponent<Workplace>();
		_districtBuilding = GetComponent<DistrictBuilding>();
		_lackOfResourcesStatus = GetComponent<LackOfResourcesStatus>();
		_noHaulingPostStatus = GetComponent<NoHaulingPostStatus>();
	}

	public void OnEnterFinishedState()
	{
		_lackOfResourcesStatus.Initialize(CheckIfSupplyIsUnavailable);
		if (!_workplace)
		{
			_noHaulingPostStatus.Initialize(() => true);
		}
	}

	public void OnExitFinishedState()
	{
		_lackOfResourcesStatus.Disable();
		if (!_workplace)
		{
			_noHaulingPostStatus.Disable();
		}
	}

	private bool CheckIfSupplyIsUnavailable()
	{
		if (((bool)_workplace && _workplace.NumberOfAssignedWorkers == 0) || !_districtBuilding.District || _goodConsumingBuilding.CanUse)
		{
			return false;
		}
		DistrictInventoryRegistry component = _districtBuilding.District.GetComponent<DistrictInventoryRegistry>();
		return CheckIfSupplyIsUnavailable(component);
	}

	private bool CheckIfSupplyIsUnavailable(DistrictInventoryRegistry inventoryRegistry)
	{
		foreach (ConsumedGoodSpec consumedGood in _goodConsumingBuilding.ConsumedGoods)
		{
			if (inventoryRegistry.ActiveInventoriesWithStock(consumedGood.GoodId).Count == 0)
			{
				return true;
			}
		}
		return false;
	}
}
