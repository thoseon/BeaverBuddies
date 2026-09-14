using Timberborn.BaseComponentSystem;
using Timberborn.CharacterNavigation;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using UnityEngine;

namespace Timberborn.Carrying;

internal class GoodCarrierCapacityReserver : BaseComponent, IAwakableComponent, IPostLoadableEntity
{
	private GoodCarrier _goodCarrier;

	private GoodReserver _goodReserver;

	private Navigator _navigator;

	private Citizen _citizen;

	public void Awake()
	{
		_goodCarrier = GetComponent<GoodCarrier>();
		_goodReserver = GetComponent<GoodReserver>();
		_navigator = GetComponent<Navigator>();
		_citizen = GetComponent<Citizen>();
	}

	public void PostLoadEntity()
	{
		if (_goodCarrier.IsCarrying && !_goodReserver.HasReservedCapacity && !ReserveCapacityForCarrier())
		{
			Debug.Log("Emptying hands due to failed reservation.");
			_goodCarrier.EmptyHands();
		}
	}

	public bool ReserveCapacityForCarrier()
	{
		Inventory inventory = FindInventoryForCarriedGoods();
		if ((bool)inventory)
		{
			_goodReserver.ReserveCapacity(inventory, _goodCarrier.CarriedGood.GoodAmount);
			return true;
		}
		return false;
	}

	private Inventory FindInventoryForCarriedGoods()
	{
		Vector3 start = _navigator.CurrentAccessOrPosition();
		GoodAmount goodAmount = _goodCarrier.CarriedGood.GoodAmount;
		float closestDistance;
		if (_citizen.HasAssignedDistrict)
		{
			return _citizen.AssignedDistrict.GetComponent<DistrictInventoryPicker>().ClosestInventoryWithCapacity(start, goodAmount, out closestDistance);
		}
		return null;
	}
}
