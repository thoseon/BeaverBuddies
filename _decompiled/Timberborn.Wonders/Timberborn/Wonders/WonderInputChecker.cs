using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.GameDistricts;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.WorkSystem;
using Timberborn.Workshops;

namespace Timberborn.Wonders;

internal class WonderInputChecker : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private Workplace _workplace;

	private Wonder _wonder;

	private WonderInventory _wonderInventory;

	private DistrictBuilding _districtBuilding;

	private LackOfResourcesStatus _lackOfResourcesStatus;

	private bool _inputUnavailable;

	public void Awake()
	{
		_workplace = GetComponent<Workplace>();
		_wonder = GetComponent<Wonder>();
		_wonderInventory = GetComponent<WonderInventory>();
		_districtBuilding = GetComponent<DistrictBuilding>();
		_lackOfResourcesStatus = GetComponent<LackOfResourcesStatus>();
	}

	public void OnEnterFinishedState()
	{
		_lackOfResourcesStatus.Initialize(() => _inputUnavailable);
		_wonderInventory.Inventory.InventoryChanged += OnInventoryChanged;
		_workplace.WorkerAssigned += OnWorkerChanged;
		_workplace.WorkerUnassigned += OnWorkerChanged;
		CheckIfInputIsUnavailable();
	}

	public void OnExitFinishedState()
	{
		_lackOfResourcesStatus.Disable();
		_wonderInventory.Inventory.InventoryChanged -= OnInventoryChanged;
		_workplace.WorkerAssigned -= OnWorkerChanged;
		_workplace.WorkerUnassigned -= OnWorkerChanged;
	}

	private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
	{
		CheckIfInputIsUnavailable();
	}

	private void OnWorkerChanged(object sender, WorkerChangedEventArgs e)
	{
		CheckIfInputIsUnavailable();
	}

	private void CheckIfInputIsUnavailable()
	{
		if (_workplace.NumberOfAssignedWorkers == 0 || !_districtBuilding.District)
		{
			_inputUnavailable = false;
		}
		else
		{
			_inputUnavailable = AreGoodsUnavailable();
		}
	}

	private bool AreGoodsUnavailable()
	{
		DistrictInventoryRegistry component = _districtBuilding.District.GetComponent<DistrictInventoryRegistry>();
		if (!_wonder.CanBeActivated())
		{
			foreach (GoodAmountSpec requiredGood in _wonderInventory.RequiredGoods)
			{
				if (component.ActiveInventoriesWithStock(requiredGood.Id).Count == 0)
				{
					return true;
				}
			}
		}
		return false;
	}
}
