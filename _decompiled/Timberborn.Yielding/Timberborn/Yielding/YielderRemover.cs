using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Carrying;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.Persistence;
using Timberborn.ReservableSystem;
using Timberborn.WorkSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.Yielding;

public class YielderRemover : BaseComponent, IAwakableComponent, IPersistentEntity, IDeletableEntity, IPostInitializableEntity
{
	private static readonly ComponentKey YielderRemoverKey = new ComponentKey("YielderRemover");

	private static readonly PropertyKey<Yielder> ReservedYielderKey = new PropertyKey<Yielder>("ReservedYielder");

	private static readonly PropertyKey<GoodAmount> ReservedYieldKey = new PropertyKey<GoodAmount>("ReservedYield");

	private readonly GoodAmountSerializer _goodAmountSerializer;

	private readonly ReferenceSerializer _referenceSerializer;

	private GoodAmount _reservedYield;

	private GoodCarrier _goodCarrier;

	public Yielder ReservedYielder { get; private set; }

	public bool HasReservedYielder => ReservedYielder;

	public event EventHandler<YieldReservationCompletedEventArgs> YieldReservationCompleted;

	public YielderRemover(GoodAmountSerializer goodAmountSerializer, ReferenceSerializer referenceSerializer)
	{
		_goodAmountSerializer = goodAmountSerializer;
		_referenceSerializer = referenceSerializer;
	}

	public void Awake()
	{
		_goodCarrier = GetComponent<GoodCarrier>();
		GetComponent<Worker>().GotUnemployed += delegate
		{
			Unreserve();
		};
	}

	public void PostInitializeEntity()
	{
		ResolveLoadedReservation();
	}

	public void DeleteEntity()
	{
		Unreserve();
	}

	public void ReserveForRemoval(Yielder yielder, GoodAmount yield)
	{
		Unreserve();
		yielder.GetComponent<Reservable>().Reserve();
		ReservedYielder = yielder;
		_reservedYield = yield;
	}

	public void Unreserve()
	{
		if (HasReservedYielder)
		{
			ReservedYielder.GetComponent<Reservable>().Unreserve();
		}
		ReservedYielder = null;
	}

	public void CompleteReservation()
	{
		if (_reservedYield.Amount > 0)
		{
			_goodCarrier.PutGoodsInHands(new CarriedGood(_reservedYield, CarriedGoodType.Uncountable));
		}
		ReservedYielder.DecreaseYield(_reservedYield);
		YieldReservationCompleted?.Invoke(this, new YieldReservationCompletedEventArgs(_reservedYield, ReservedYielder));
		Unreserve();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (HasReservedYielder)
		{
			IObjectSaver component = entitySaver.GetComponent(YielderRemoverKey);
			component.Set(ReservedYielderKey, ReservedYielder, _referenceSerializer.Of<Yielder>());
			component.Set(ReservedYieldKey, _reservedYield, _goodAmountSerializer);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(YielderRemoverKey, out var objectLoader) && objectLoader.GetObsoletable(ReservedYieldKey, _goodAmountSerializer, out var value) && objectLoader.GetObsoletable(ReservedYielderKey, _referenceSerializer.Of<Yielder>(), out var value2))
		{
			ReserveForRemoval(value2, value);
		}
	}

	private void ResolveLoadedReservation()
	{
		if ((bool)ReservedYielder)
		{
			GoodAmount yield = ReservedYielder.Yield;
			int amount = yield.Amount;
			if (_reservedYield.Amount > amount)
			{
				Debug.LogWarning($"Reducing {base.Name}'s reservation of {ReservedYielder} " + $"from {_reservedYield.Amount} to {amount}");
				GoodAmount reservedYield = new GoodAmount(yield.GoodId, amount);
				_reservedYield = reservedYield;
			}
			ReserveForRemoval(ReservedYielder, _reservedYield);
		}
	}
}
