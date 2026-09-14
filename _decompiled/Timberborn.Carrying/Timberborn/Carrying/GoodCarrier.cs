using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BonusSystem;
using Timberborn.CharacterMovementSystem;
using Timberborn.Characters;
using Timberborn.Common;
using Timberborn.Goods;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.Carrying;

public class GoodCarrier : BaseComponent, IAwakableComponent, IPersistentEntity, IMovementSpeedAffector
{
	private static readonly string CarryingCapacityBonusId = "CarryingCapacity";

	private static readonly ComponentKey GoodCarrierKey = new ComponentKey("GoodCarrier");

	private static readonly PropertyKey<CarriedGood> CarriedGoodKey = new PropertyKey<CarriedGood>("CarriedGood");

	private readonly CarriedGoodSerializer _carriedGoodSerializer;

	private readonly GoodAmountSerializer _goodAmountSerializer;

	private BonusManager _bonusManager;

	private GoodCarrierSpec _goodCarrierSpec;

	public CarriedGood CarriedGood { get; private set; } = CarriedGood.Empty;

	public int LiftingCapacity
	{
		get
		{
			double num = Math.Round(_bonusManager.Multiplier(CarryingCapacityBonusId), 2);
			return (int)((double)_goodCarrierSpec.BaseLiftingCapacity * num);
		}
	}

	public bool IsCarrying => !CarriedGood.IsEmpty;

	public string CarriedGoodId => CarriedGood.GoodAmount.GoodId;

	public bool IsMovementSlowed => IsCarrying;

	public event EventHandler<CarriedGoodsChangedEventArgs> CarriedGoodsChanged;

	internal GoodCarrier(CarriedGoodSerializer carriedGoodSerializer, GoodAmountSerializer goodAmountSerializer)
	{
		_carriedGoodSerializer = carriedGoodSerializer;
		_goodAmountSerializer = goodAmountSerializer;
	}

	public void Awake()
	{
		GetComponent<Character>().Died += OnDied;
		_bonusManager = GetComponent<BonusManager>();
		_goodCarrierSpec = GetComponent<GoodCarrierSpec>();
	}

	public void PutGoodsInHands(CarriedGood carriedGood)
	{
		if (!IsCarrying)
		{
			SetCarriedGood(carriedGood);
			return;
		}
		throw new InvalidOperationException($"Tried to put {carriedGood.GoodAmount} in {base.Name}'s " + "hands but they are already carrying something");
	}

	public void EmptyHands()
	{
		SetCarriedGood(CarriedGood.Empty);
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (IsCarrying)
		{
			entitySaver.GetComponent(GoodCarrierKey).Set(CarriedGoodKey, CarriedGood, _carriedGoodSerializer);
		}
	}

	[BackwardCompatible(2026, 5, 6, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(GoodCarrierKey, out var objectLoader))
		{
			GoodAmount value2;
			if (objectLoader.Has(CarriedGoodKey))
			{
				if (objectLoader.GetObsoletable(CarriedGoodKey, _carriedGoodSerializer, out var value))
				{
					PutGoodsInHands(value);
				}
				else
				{
					EmptyHands();
				}
			}
			else if (objectLoader.GetObsoletable(new PropertyKey<GoodAmount>("CarriedGoods"), _goodAmountSerializer, out value2))
			{
				PropertyKey<bool> key = new PropertyKey<bool>("CountGoodAsAvailable");
				CarriedGoodType type = ((objectLoader.Has(key) && objectLoader.Get(key)) ? CarriedGoodType.Available : CarriedGoodType.Unavailable);
				PutGoodsInHands(new CarriedGood(value2, type));
			}
			else
			{
				EmptyHands();
			}
		}
		else
		{
			EmptyHands();
		}
	}

	private void OnDied(object sender, EventArgs e)
	{
		EmptyHands();
	}

	private void SetCarriedGood(CarriedGood carriedGood)
	{
		CarriedGood = carriedGood;
		CarriedGoodsChanged?.Invoke(this, new CarriedGoodsChangedEventArgs(carriedGood));
	}
}
