using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.Buildings;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Persistence;
using Timberborn.ResourceCountingSystem;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.GoodConsumingBuildingSystem;

public class GoodConsumingBuilding : TickableComponent, IAwakableComponent, IFinishedStateListener, IPostInitializableEntity, IPersistentEntity, IRegisteredComponent, IBuildingEfficiencyProvider, IFinishedPausable, IGoodProcessor
{
	private static readonly ComponentKey GoodConsumingBuildingKey = new ComponentKey("GoodConsumingBuilding");

	private static readonly ListKey<float> SuppliesLeftKey = new ListKey<float>("SuppliesLeft");

	private readonly IDayNightCycle _dayNightCycle;

	private BlockableObject _blockableObject;

	private GoodConsumingBuildingSpec _goodConsumingBuildingSpec;

	private readonly List<GoodConsumingToggle> _toggles = new List<GoodConsumingToggle>();

	private readonly List<float> _suppliesLeft = new List<float>();

	private readonly List<GoodAmount> _processedGoods = new List<GoodAmount>();

	public Inventory Inventory { get; private set; }

	public bool IsConsuming { get; private set; }

	public bool ConsumptionPaused { get; private set; }

	public float MaximumWorkingTime { get; private set; }

	public bool CanUse => CanUseInternal();

	public float Efficiency => CanUse ? 1 : 0;

	public ImmutableArray<ConsumedGoodSpec> ConsumedGoods => _goodConsumingBuildingSpec.ConsumedGoods;

	public ReadOnlyList<GoodAmount> ProcessedGoods => _processedGoods.AsReadOnlyList();

	public GoodConsumingBuilding(IDayNightCycle dayNightCycle)
	{
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
		_goodConsumingBuildingSpec = GetComponent<GoodConsumingBuildingSpec>();
		for (int i = 0; i < ConsumedGoods.Length; i++)
		{
			_suppliesLeft.Add(0f);
		}
		MaximumWorkingTime = (float)_goodConsumingBuildingSpec.FullInventoryWorkHours + 1f / ConsumedGoods.Max((ConsumedGoodSpec good) => good.GoodPerHour);
		DisableComponent();
	}

	public void InitializeInventory(Inventory inventory)
	{
		Asserts.FieldIsNull(this, Inventory, "Inventory");
		Inventory = inventory;
	}

	public override void Tick()
	{
		UpdateIsConsuming();
		ConsumeSupplies();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		Inventory.Enable();
		Inventory.InventoryStockChanged += OnInventoryStockChanged;
	}

	public void OnExitFinishedState()
	{
		Inventory.InventoryStockChanged -= OnInventoryStockChanged;
		Inventory.Disable();
		DisableComponent();
	}

	public void PostInitializeEntity()
	{
		UpdateIsConsuming();
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(GoodConsumingBuildingKey).Set(SuppliesLeftKey, _suppliesLeft);
	}

	[BackwardCompatible(2025, 10, 23, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(GoodConsumingBuildingKey);
		if (component.Has(SuppliesLeftKey))
		{
			List<float> list = component.Get(SuppliesLeftKey);
			for (int i = 0; i < _suppliesLeft.Count && i < list.Count; i++)
			{
				_suppliesLeft[i] = list[i];
			}
		}
		else
		{
			float value = component.Get(new PropertyKey<float>("SupplyLeft"));
			for (int j = 0; j < _suppliesLeft.Count; j++)
			{
				_suppliesLeft[j] = value;
			}
		}
	}

	public GoodConsumingToggle GetGoodConsumingToggle()
	{
		GoodConsumingToggle goodConsumingToggle = new GoodConsumingToggle();
		_toggles.Add(goodConsumingToggle);
		goodConsumingToggle.StateChanged += delegate
		{
			UpdateConsumptionState();
		};
		return goodConsumingToggle;
	}

	public float HoursUntilNoSupply()
	{
		float num = float.MaxValue;
		for (int i = 0; i < ConsumedGoods.Length; i++)
		{
			ConsumedGoodSpec consumedGoodSpec = ConsumedGoods[i];
			float val = ((float)Inventory.UnreservedAmountInStock(consumedGoodSpec.GoodId) + _suppliesLeft[i]) / consumedGoodSpec.GoodPerHour;
			num = Math.Min(num, val);
		}
		return num;
	}

	private void OnInventoryStockChanged(object sender, InventoryStockChangedEventArgs e)
	{
		UpdateProcessingGoods();
	}

	private void UpdateProcessingGoods()
	{
		_processedGoods.Clear();
		for (int i = 0; i < _suppliesLeft.Count; i++)
		{
			if (_suppliesLeft[i] > 0f)
			{
				string goodId = ConsumedGoods[i].GoodId;
				_processedGoods.Add(new GoodAmount(goodId, 1));
			}
		}
	}

	private bool CanUseInternal()
	{
		for (int i = 0; i < ConsumedGoods.Length; i++)
		{
			if (_suppliesLeft[i] <= 0f && Inventory.UnreservedAmountInStock(ConsumedGoods[i].GoodId) == 0)
			{
				return false;
			}
		}
		return true;
	}

	private void UpdateConsumptionState()
	{
		ConsumptionPaused = _toggles.FastAny((GoodConsumingToggle toggle) => toggle.Paused);
	}

	private void UpdateIsConsuming()
	{
		IsConsuming = !ConsumptionPaused && _blockableObject.IsUnblocked && HasSupplies();
		UpdateProcessingGoods();
	}

	private void ConsumeSupplies()
	{
		if (IsConsuming)
		{
			for (int i = 0; i < _suppliesLeft.Count; i++)
			{
				_suppliesLeft[i] -= _dayNightCycle.FixedDeltaTimeInHours * ConsumedGoods[i].GoodPerHour;
			}
		}
	}

	private bool HasSupplies()
	{
		for (int i = 0; i < _suppliesLeft.Count; i++)
		{
			if (_suppliesLeft[i] <= 0f)
			{
				string goodId = ConsumedGoods[i].GoodId;
				if (Inventory.UnreservedAmountInStock(goodId) <= 0)
				{
					return false;
				}
				Inventory.TakeConsumed(new GoodAmount(goodId, 1));
				_suppliesLeft[i] = 1f;
			}
		}
		return true;
	}
}
