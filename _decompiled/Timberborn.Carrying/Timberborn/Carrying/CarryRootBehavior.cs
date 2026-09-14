using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Navigation;
using Timberborn.WalkingSystem;
using Timberborn.WorkSystem;

namespace Timberborn.Carrying;

public class CarryRootBehavior : RootBehavior, IAwakableComponent, IJobBehavior
{
	private readonly CarryAmountCalculator _carryAmountCalculator;

	private GoodCarrier _goodCarrier;

	private GoodReserver _goodReserver;

	private GoodCarrierCapacityReserver _goodCarrierCapacityReserver;

	private WalkToAccessibleExecutor _walkToAccessibleExecutor;

	public CarryRootBehavior(CarryAmountCalculator carryAmountCalculator)
	{
		_carryAmountCalculator = carryAmountCalculator;
	}

	public void Awake()
	{
		_goodCarrier = GetComponent<GoodCarrier>();
		_goodReserver = GetComponent<GoodReserver>();
		_goodCarrierCapacityReserver = GetComponent<GoodCarrierCapacityReserver>();
		_walkToAccessibleExecutor = GetComponent<WalkToAccessibleExecutor>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (TryToDeliver(out var decision))
		{
			return decision;
		}
		if (TryToRetrieve(out var decision2))
		{
			return decision2;
		}
		if (_goodReserver.HasReservedStock)
		{
			_goodReserver.UnreserveStock();
		}
		return Decision.ReleaseNow();
	}

	private bool TryToDeliver(out Decision decision)
	{
		if (_goodCarrier.IsCarrying)
		{
			if (!_goodReserver.HasReservedCapacity && !_goodCarrierCapacityReserver.ReserveCapacityForCarrier())
			{
				_goodCarrier.EmptyHands();
				decision = Decision.ReleaseNow();
			}
			else
			{
				Accessible enabledComponent = _goodReserver.CapacityReservation.Inventory.GetEnabledComponent<Accessible>();
				decision = _walkToAccessibleExecutor.Launch(enabledComponent) switch
				{
					ExecutorStatus.Success => CompleteDelivery(), 
					ExecutorStatus.Failure => TryFallbackDelivery(), 
					ExecutorStatus.Running => Decision.ReturnWhenFinished(_walkToAccessibleExecutor), 
					_ => throw new ArgumentOutOfRangeException(), 
				};
			}
			return true;
		}
		decision = default(Decision);
		return false;
	}

	private Decision CompleteDelivery()
	{
		GoodReservation capacityReservation = _goodReserver.CapacityReservation;
		_goodReserver.UnreserveCapacity();
		if (capacityReservation.Inventory.HasUnreservedCapacity(capacityReservation.GoodAmount))
		{
			if (_goodCarrier.CarriedGood.Type == CarriedGoodType.Uncountable)
			{
				capacityReservation.Inventory.GiveProduced(capacityReservation.GoodAmount);
			}
			else
			{
				capacityReservation.Inventory.GiveExisting(capacityReservation.GoodAmount);
			}
			_goodCarrier.EmptyHands();
			return Decision.ReleaseNow();
		}
		return Decision.ReleaseNextTick();
	}

	private Decision TryFallbackDelivery()
	{
		if (!_goodCarrierCapacityReserver.ReserveCapacityForCarrier())
		{
			return TryForceDelivery();
		}
		return Decision.ReturnNextTick();
	}

	private Decision TryForceDelivery()
	{
		Accessible enabledComponent = _goodReserver.CapacityReservation.Inventory.GetEnabledComponent<Accessible>();
		return _walkToAccessibleExecutor.LaunchIgnoringAccessibleValidity(enabledComponent) switch
		{
			ExecutorStatus.Success => CompleteDelivery(), 
			ExecutorStatus.Failure => UnreserveCapacity(), 
			ExecutorStatus.Running => Decision.ReturnWhenFinished(_walkToAccessibleExecutor), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private Decision UnreserveCapacity()
	{
		_goodReserver.UnreserveCapacity();
		return Decision.ReleaseNextTick();
	}

	private bool TryToRetrieve(out Decision decision)
	{
		if (_goodReserver.HasReservedCapacity)
		{
			if (_goodReserver.HasReservedStock)
			{
				Accessible enabledComponent = _goodReserver.StockReservation.Inventory.GetEnabledComponent<Accessible>();
				decision = _walkToAccessibleExecutor.LaunchIgnoringAccessibleValidity(enabledComponent) switch
				{
					ExecutorStatus.Success => CompleteRetrieval(), 
					ExecutorStatus.Failure => UnreserveStock(), 
					ExecutorStatus.Running => Decision.ReturnWhenFinished(_walkToAccessibleExecutor), 
					_ => throw new ArgumentOutOfRangeException(), 
				};
			}
			else
			{
				_goodReserver.UnreserveCapacity();
				decision = Decision.ReleaseNextTick();
			}
			return true;
		}
		decision = default(Decision);
		return false;
	}

	private Decision CompleteRetrieval()
	{
		GoodReservation stockReservation = _goodReserver.StockReservation;
		_goodReserver.UnreserveStock();
		GoodAmount goodAmount = (stockReservation.FixedAmount ? stockReservation.GoodAmount : RecalculateAmountToRetrieve(stockReservation));
		if (stockReservation.ConsumeGood)
		{
			stockReservation.Inventory.TakeConsumed(goodAmount);
		}
		else
		{
			stockReservation.Inventory.TakeExisting(goodAmount);
		}
		CarriedGood carriedGood = CreateCarriedGood(goodAmount, stockReservation, _goodReserver.CapacityReservation);
		_goodCarrier.PutGoodsInHands(carriedGood);
		if (!TryToDeliver(out var decision))
		{
			return Decision.ReturnNextTick();
		}
		return decision;
	}

	private Decision UnreserveStock()
	{
		_goodReserver.UnreserveStock();
		return Decision.ReleaseNextTick();
	}

	private GoodAmount RecalculateAmountToRetrieve(GoodReservation goodReservation)
	{
		GoodReservation capacityReservation = _goodReserver.CapacityReservation;
		_goodReserver.UnreserveCapacity();
		string goodId = goodReservation.GoodAmount.GoodId;
		GoodAmount goodAmount = _carryAmountCalculator.AmountToCarry(_goodCarrier.LiftingCapacity, goodId, capacityReservation.Inventory, goodReservation.Inventory);
		_goodReserver.ReserveCapacity(capacityReservation.Inventory, goodAmount);
		return goodAmount;
	}

	private static CarriedGood CreateCarriedGood(GoodAmount goodAmount, GoodReservation stockReservation, GoodReservation capacityReservation)
	{
		if (stockReservation.ConsumeGood)
		{
			return new CarriedGood(goodAmount, CarriedGoodType.Uncountable);
		}
		Inventory inventory = capacityReservation.Inventory;
		if (inventory != null && inventory.Gives(goodAmount.GoodId))
		{
			return new CarriedGood(goodAmount, CarriedGoodType.Available);
		}
		return new CarriedGood(goodAmount, CarriedGoodType.Unavailable);
	}
}
