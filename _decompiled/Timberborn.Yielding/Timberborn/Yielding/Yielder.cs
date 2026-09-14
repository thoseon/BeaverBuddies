using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Goods;
using Timberborn.Persistence;
using Timberborn.ReservableSystem;
using Timberborn.TemplateSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.Yielding;

public class Yielder : BaseComponent, IAwakableComponent, IPersistentEntity, INamedComponent
{
	private static readonly ComponentKey YielderKey = new ComponentKey("Yielder");

	private static readonly PropertyKey<GoodAmount> YieldKey = new PropertyKey<GoodAmount>("Yield");

	private readonly GoodAmountSerializer _goodAmountSerializer;

	private BlockObject _blockObject;

	private BlockObjectCenter _blockObjectCenter;

	private InstantiatedTemplate _instantiatedTemplate;

	private GoodAmount _yield;

	private GoodAmount _initialYield;

	public Reservable Reservable { get; private set; }

	public YielderSpec YielderSpec { get; private set; }

	public IRemoveYieldStrategy RemoveYieldStrategy { get; private set; }

	public string Animation { get; private set; }

	public string ComponentName => YielderSpec.YielderComponentName;

	public float RemovalTimeInHours => YielderSpec.RemovalTimeInHours;

	public bool IsYieldRemoved => _yield.Amount == 0;

	public Vector3 CenterPosition => _blockObjectCenter.WorldCenterGrounded;

	public Vector3Int Coordinates => _blockObject.Coordinates;

	public bool IsYielding => Yield.Amount > 0;

	public int InstantiationOrder => _instantiatedTemplate.InstantiationOrder;

	public GoodAmount Yield
	{
		get
		{
			if (!base.Enabled)
			{
				return new GoodAmount(_yield.GoodId, 0);
			}
			return _yield;
		}
	}

	public event EventHandler YieldAdded;

	public event EventHandler YieldDecreased;

	public Yielder(GoodAmountSerializer goodAmountSerializer)
	{
		_goodAmountSerializer = goodAmountSerializer;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_blockObjectCenter = GetComponent<BlockObjectCenter>();
		_instantiatedTemplate = GetComponent<InstantiatedTemplate>();
		Reservable = GetComponent<Reservable>();
		DisableComponent();
	}

	public void Initialize(YielderSpec yielderSpec, GoodAmount yield, IRemoveYieldStrategy removeYieldStrategy, string animation)
	{
		YielderSpec = yielderSpec;
		RemoveYieldStrategy = removeYieldStrategy;
		Animation = animation;
		_initialYield = yield;
		_yield = yield;
	}

	public void ResetYield()
	{
		Enable();
		_yield = _initialYield;
		YieldAdded?.Invoke(this, EventArgs.Empty);
	}

	public void DecreaseYield(GoodAmount decreasedYield)
	{
		_yield = new GoodAmount(_yield.GoodId, _yield.Amount - decreasedYield.Amount);
		YieldDecreased?.Invoke(this, EventArgs.Empty);
	}

	public void RemoveRemainingYield()
	{
		SetYieldToZero();
	}

	public void Enable()
	{
		EnableComponent();
	}

	public void Disable()
	{
		DisableComponent();
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(YielderKey, ComponentName).Set(YieldKey, _yield, _goodAmountSerializer);
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(YielderKey, ComponentName, out var objectLoader) && objectLoader.GetObsoletable(YieldKey, _goodAmountSerializer, out var value) && value.GoodId == YielderSpec.Yield.Id)
		{
			_yield = value;
		}
	}

	private void SetYieldToZero()
	{
		Enable();
		_yield = new GoodAmount(_initialYield.GoodId, 0);
	}
}
