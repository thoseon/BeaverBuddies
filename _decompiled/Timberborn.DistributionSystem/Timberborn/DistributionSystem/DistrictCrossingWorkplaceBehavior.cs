using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Carrying;
using Timberborn.Common;
using Timberborn.Emptying;
using Timberborn.TimeSystem;
using Timberborn.WorkSystem;
using UnityEngine;

namespace Timberborn.DistributionSystem;

internal class DistrictCrossingWorkplaceBehavior : WorkplaceBehavior, IAwakableComponent
{
	private static readonly float PrioritizeEmptyingDistanceSquared = 4f;

	private readonly IDayNightCycle _dayNightCycle;

	private DistrictCrossing _districtCrossing;

	private DistrictCrossingInventory _districtCrossingInventory;

	private readonly List<DistributableGood> _linkedDistrictDistributableGoods = new List<DistributableGood>();

	public DistrictCrossingWorkplaceBehavior(IDayNightCycle dayNightCycle)
	{
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		_districtCrossing = GetComponent<DistrictCrossing>();
		_districtCrossingInventory = GetComponent<DistrictCrossingInventory>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (IsCloseToDistrictCrossing(agent))
		{
			if (TryEmptying(agent))
			{
				return Decision.ReleaseNextTick();
			}
			if (TryExport(agent))
			{
				return Decision.ReleaseNextTick();
			}
		}
		else
		{
			if (TryExport(agent))
			{
				return Decision.ReleaseNextTick();
			}
			if (TryEmptying(agent))
			{
				return Decision.ReleaseNextTick();
			}
		}
		return Decision.ReleaseNow();
	}

	private bool IsCloseToDistrictCrossing(BehaviorAgent agent)
	{
		Vector2 vector = agent.Transform.position.XZ();
		Vector2 vector2 = _districtCrossing.Transform.position.XZ();
		return (vector - vector2).sqrMagnitude < PrioritizeEmptyingDistanceSquared;
	}

	private bool TryEmptying(BehaviorAgent agent)
	{
		if (_districtCrossingInventory.Inventory.OutputGoods.Count > 0)
		{
			return agent.GetComponent<EmptyingStarter>().StartEmptying(_districtCrossingInventory.Inventory);
		}
		return false;
	}

	private bool TryExport(BehaviorAgent agent)
	{
		if (_districtCrossing.CanExport)
		{
			_districtCrossing.GetLinkedDistrictDistributableGoods(_linkedDistrictDistributableGoods);
			bool result = TryExportDistributableGoods(agent);
			_linkedDistrictDistributableGoods.Clear();
			return result;
		}
		return false;
	}

	private bool TryExportDistributableGoods(BehaviorAgent agent)
	{
		for (int i = 0; i < _linkedDistrictDistributableGoods.Count; i++)
		{
			if (TryExportGood(agent, _linkedDistrictDistributableGoods[i]))
			{
				return true;
			}
		}
		return false;
	}

	private bool TryExportGood(BehaviorAgent agent, DistributableGood linkedDistributableGood)
	{
		string goodId = linkedDistributableGood.GoodId;
		DistributableGood myDistributableGood = _districtCrossing.GetMyDistributableGood(goodId);
		if (_districtCrossing.CanExportGood(myDistributableGood, linkedDistributableGood))
		{
			int amountToExport = _districtCrossing.GetAmountToExport(myDistributableGood, linkedDistributableGood);
			if (_districtCrossingInventory.Inventory.UnreservedAmountInStock(goodId) > 0)
			{
				_districtCrossingInventory.TransferStock(goodId, amountToExport);
			}
			else if (TryStartCarrying(agent, goodId, amountToExport, linkedDistributableGood))
			{
				return true;
			}
		}
		return false;
	}

	private bool TryStartCarrying(BehaviorAgent agent, string goodId, int amountToBring, DistributableGood linkedDistributableGood)
	{
		int val = _districtCrossingInventory.Inventory.UnreservedCapacity(goodId);
		int num = Math.Min(amountToBring, val);
		if (num > 0 && agent.GetComponent<CarrierInventoryFinder>().TryCarryFromAnyInventoryLimited(goodId, _districtCrossingInventory.Inventory, num))
		{
			linkedDistributableGood.UpdateLastImportTimestamp(_dayNightCycle.PartialDayNumber);
			return true;
		}
		return false;
	}
}
