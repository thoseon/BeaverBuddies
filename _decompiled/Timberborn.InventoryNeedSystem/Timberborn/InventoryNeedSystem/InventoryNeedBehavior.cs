using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.Common;
using Timberborn.Effects;
using Timberborn.EnterableSystem;
using Timberborn.GameDistricts;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.NeedBehaviorSystem;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;
using Timberborn.SingletonSystem;
using Timberborn.WalkingSystem;
using UnityEngine;

namespace Timberborn.InventoryNeedSystem;

public class InventoryNeedBehavior : NeedBehavior, IAwakableComponent, IFinishedStateListener
{
	private readonly IGoodService _goodService;

	private readonly EventBus _eventBus;

	private BuildingAccessible _buildingAccessible;

	private Enterable _enterable;

	private Inventory _inventory;

	private DistrictBuilding _districtBuilding;

	private DistrictNeedBehaviorService _districtNeedBehaviorService;

	private readonly Dictionary<string, IReadOnlyList<InstantEffectSpec>> _availableConsumables = new Dictionary<string, IReadOnlyList<InstantEffectSpec>>();

	public InventoryNeedBehavior(IGoodService goodService, EventBus eventBus)
	{
		_goodService = goodService;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_enterable = GetComponent<Enterable>();
		_buildingAccessible = GetComponent<BuildingAccessible>();
		_districtBuilding = GetComponent<DistrictBuilding>();
		DisableComponent();
	}

	public void Initialize(Inventory inventory)
	{
		Asserts.FieldIsNull(this, _inventory, "_inventory");
		_inventory = inventory;
		_inventory.InventoryChanged += OnInventoryChanged;
	}

	public override Vector3? ActionPosition(NeedManager needManager)
	{
		if (_enterable.CanReserveSlot)
		{
			Vector3? unblockedSingleAccess = _buildingAccessible.Accessible.UnblockedSingleAccess;
			if (unblockedSingleAccess.HasValue)
			{
				return unblockedSingleAccess.GetValueOrDefault();
			}
		}
		return null;
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		GoodReserver component = agent.GetComponent<GoodReserver>();
		if (!_inventory.Enabled)
		{
			if (component.StockReservation.Inventory != _inventory)
			{
				return Decision.ReleaseNextTick();
			}
			return UnreserveGood(component);
		}
		if (component.StockReservation.Inventory != _inventory)
		{
			GoodAmount good = FindMostOptimalGood(agent.GetComponent<Appraiser>(), _inventory);
			if (good.Amount <= 0)
			{
				return Decision.ReleaseNextTick();
			}
			component.ReserveExactStockAmount(_inventory, good);
		}
		WalkInsideExecutor component2 = agent.GetComponent<WalkInsideExecutor>();
		return component2.LaunchIgnoringAccessibleValidity(_enterable) switch
		{
			ExecutorStatus.Success => ConsumeGood(agent.GetComponent<NeedManager>(), component), 
			ExecutorStatus.Failure => UnreserveGood(component), 
			ExecutorStatus.Running => Decision.ReturnWhenFinished(component2), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	public void OnEnterFinishedState()
	{
		if ((bool)_inventory)
		{
			_districtBuilding.ReassignedDistrict += OnReassignedDistrict;
			EnableComponent();
		}
	}

	public void OnExitFinishedState()
	{
		if ((bool)_inventory)
		{
			_districtBuilding.ReassignedDistrict -= OnReassignedDistrict;
			RemoveDistrictNeedBehaviorService();
			DisableComponent();
		}
	}

	private GoodAmount FindMostOptimalGood(Appraiser appraiser, Inventory inventory)
	{
		IEnumerable<GoodAmount> enumerable = inventory.UnreservedTakeableStock();
		float num = 0f;
		GoodAmount result = default(GoodAmount);
		foreach (GoodAmount item in enumerable)
		{
			GoodAmount goodAmount = new GoodAmount(item.GoodId, 1);
			float num2 = AppraiseGood(goodAmount, appraiser);
			if (num2 > num)
			{
				num = num2;
				result = goodAmount;
			}
		}
		return result;
	}

	private static Decision UnreserveGood(GoodReserver goodReserver)
	{
		goodReserver.UnreserveStock();
		return Decision.ReleaseNextTick();
	}

	private Decision ConsumeGood(NeedManager needManager, GoodReserver goodReserver)
	{
		GoodAmount goodAmount = goodReserver.StockReservation.GoodAmount;
		RemoveGoodFromInventory(goodReserver);
		ApplyConsumptionEffects(needManager, goodAmount);
		_eventBus.Post(new GoodConsumedEvent(goodAmount.GoodId));
		return Decision.ReturnNextTick();
	}

	private static void RemoveGoodFromInventory(GoodReserver goodReserver)
	{
		GoodReservation stockReservation = goodReserver.StockReservation;
		GoodAmount goodAmount = stockReservation.GoodAmount;
		goodReserver.UnreserveStock();
		stockReservation.Inventory.TakeConsumed(goodAmount);
	}

	private void ApplyConsumptionEffects(NeedManager needManager, GoodAmount goodAmount)
	{
		foreach (InstantEffectSpec consumptionEffect in _goodService.GetGood(goodAmount.GoodId).ConsumptionEffects)
		{
			needManager.ApplyEffect(InstantEffect.FromSpec(consumptionEffect, goodAmount.Amount));
		}
	}

	private float AppraiseGood(GoodAmount good, Appraiser appraiser)
	{
		IEnumerable<InstantEffect> instantEffects = GoodEffects(good);
		return appraiser.AppraiseEffects(instantEffects);
	}

	private IEnumerable<InstantEffect> GoodEffects(GoodAmount good)
	{
		return _goodService.GetGood(good.GoodId).ConsumptionEffects.Select((InstantEffectSpec effectSpec) => InstantEffect.FromSpec(effectSpec, good.Amount));
	}

	private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
	{
		GoodSpec good = _goodService.GetGood(e.GoodId);
		if (good.HasConsumptionEffects)
		{
			UpdateAvailableConsumables(good);
		}
	}

	private void UpdateAvailableConsumables(GoodSpec good)
	{
		if (_inventory.HasUnreservedStock(good.Id))
		{
			if (_availableConsumables.TryAdd(good.Id, good.ConsumptionEffects) && (bool)_districtNeedBehaviorService)
			{
				_districtNeedBehaviorService.AddNeedBehavior(good.ConsumptionEffects, this);
			}
		}
		else if (_availableConsumables.Remove(good.Id) && (bool)_districtNeedBehaviorService)
		{
			_districtNeedBehaviorService.RemoveNeedBehavior(good.ConsumptionEffects, this);
		}
	}

	private void OnReassignedDistrict(object sender, EventArgs e)
	{
		RemoveDistrictNeedBehaviorService();
		DistrictCenter district = _districtBuilding.District;
		if (!district)
		{
			return;
		}
		_districtNeedBehaviorService = district.GetComponent<DistrictNeedBehaviorService>();
		foreach (IReadOnlyList<InstantEffectSpec> value in _availableConsumables.Values)
		{
			_districtNeedBehaviorService.AddNeedBehavior(value, this);
		}
	}

	private void RemoveDistrictNeedBehaviorService()
	{
		if (!_districtNeedBehaviorService)
		{
			return;
		}
		foreach (IReadOnlyList<InstantEffectSpec> value in _availableConsumables.Values)
		{
			_districtNeedBehaviorService.RemoveNeedBehavior(value, this);
		}
		_districtNeedBehaviorService = null;
	}
}
