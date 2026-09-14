using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.BuildingsNavigation;
using Timberborn.Carrying;
using Timberborn.GameDistricts;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.ConstructionSites;

public class ConstructionJob : BaseComponent, IAwakableComponent
{
	private readonly CarryAmountCalculator _carryAmountCalculator;

	private ConstructionSite _constructionSite;

	private ConstructionSiteAccessible _constructionSiteAccessible;

	private readonly SortedSet<GoodAmount> _remainingGoods = new SortedSet<GoodAmount>(new GoodAmountComparer());

	public ConstructionJob(CarryAmountCalculator carryAmountCalculator)
	{
		_carryAmountCalculator = carryAmountCalculator;
	}

	public void Awake()
	{
		_constructionSite = GetComponent<ConstructionSite>();
		_constructionSiteAccessible = GetComponent<ConstructionSiteAccessible>();
	}

	public (Behavior, Decision) StartConstructionJob(BehaviorAgent agent, Accessible workplaceAccessible)
	{
		DistrictCenter district = workplaceAccessible.GetComponent<DistrictBuilding>().District;
		if (!_constructionSite.IsOn || !district || !workplaceAccessible.FindRoadToTerrainPath(_constructionSiteAccessible.Accessible, out var endOfRoad, out var _))
		{
			return (null, Decision.ReleaseNow());
		}
		if (_constructionSite.ReadyToBuild)
		{
			BuildBehavior component = agent.GetComponent<BuildBehavior>();
			Decision item = component.StartBuilding(_constructionSite);
			if (!item.ShouldReleaseNow)
			{
				return (component, item);
			}
		}
		var (inventory, neededGood) = ClosestObjectWithNeededGood(district, endOfRoad, workplaceAccessible);
		if ((bool)inventory)
		{
			StartNeededMaterialsDelivery(agent, inventory, neededGood);
			return (null, Decision.ReleaseNextTick());
		}
		return (null, Decision.ReleaseNow());
	}

	private (Inventory inventory, string goodId) ClosestObjectWithNeededGood(DistrictCenter districtCenter, Vector3 endOfRoad, Accessible workplaceAccessible)
	{
		_constructionSite.RemainingRequiredGoods(_remainingGoods);
		var (inventory, item) = ClosestInventory(districtCenter, endOfRoad, workplaceAccessible);
		if ((bool)inventory)
		{
			_constructionSite.DeactivateLackOfResourcesStatus();
			_remainingGoods.Clear();
			return (inventory: inventory, goodId: item);
		}
		if (_remainingGoods.Count > 0)
		{
			_remainingGoods.Clear();
			_constructionSite.ActivateLackOfResourcesStatus();
		}
		return (inventory: null, goodId: null);
	}

	private (Inventory inventory, string goodId) ClosestInventory(DistrictCenter districtCenter, Vector3 endOfRoad, Accessible workplaceAccessible)
	{
		DistrictInventoryPicker component = districtCenter.GetComponent<DistrictInventoryPicker>();
		foreach (GoodAmount remainingGood in _remainingGoods)
		{
			Inventory inventory = component.ClosestInventoryWithStock(endOfRoad, remainingGood.GoodId, workplaceAccessible);
			if ((bool)inventory)
			{
				return (inventory: inventory, goodId: remainingGood.GoodId);
			}
		}
		return default((Inventory, string));
	}

	private void StartNeededMaterialsDelivery(BehaviorAgent agent, Inventory inventory, string neededGood)
	{
		GoodCarrier component = agent.GetComponent<GoodCarrier>();
		GoodAmount goodAmount = _carryAmountCalculator.AmountToCarry(component.LiftingCapacity, neededGood, _constructionSite.Inventory, inventory);
		if (goodAmount.Amount > 0)
		{
			GoodReserver component2 = agent.GetComponent<GoodReserver>();
			component2.ReserveNotLessThanStockAmountConsuming(inventory, goodAmount);
			component2.ReserveCapacity(_constructionSite.Inventory, goodAmount);
		}
	}
}
