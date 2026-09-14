using System;
using System.Collections.Generic;
using System.Text;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.InventorySystem;

public class Inventory : BaseComponent, IAwakableComponent, IPersistentEntity, IAmountProvider, INamedComponent
{
	private static readonly ComponentKey InventoryKey = new ComponentKey("Inventory");

	private static readonly PropertyKey<GoodRegistry> StorageKey = new PropertyKey<GoodRegistry>("Storage");

	private readonly GoodRegistryValueSerializer _goodRegistryValueSerializer;

	private BlockableObject _blockableObject;

	private DistrictInventoryAssigner _districtInventoryAssigner;

	private readonly StorableGoodRegistry _allowedGoods = new StorableGoodRegistry();

	private GoodRegistry _storage = new GoodRegistry();

	private readonly GoodRegistry _reservedStock = new GoodRegistry();

	private readonly GoodRegistry _reservedCapacity = new GoodRegistry();

	private IGoodDisallower _goodDisallower;

	private bool _ignorableCapacity;

	public string ComponentName { get; private set; }

	public int Capacity { get; private set; }

	public bool HasUnwantedStock { get; private set; }

	public bool PublicInput { get; private set; }

	public bool PublicOutput { get; private set; }

	public int TotalAmountInStock => _storage.TotalAmount;

	public bool IsFull => TotalAmountInStock >= Capacity;

	public bool IsFullyReserved => TotalAmountInStock + _reservedCapacity.TotalAmount >= Capacity;

	public bool HasAnyUnreservedStock => _storage.TotalAmount > _reservedStock.TotalAmount;

	public bool IsEmpty => TotalAmountInStock == 0;

	public bool IsInput => _allowedGoods.HasInputGoods;

	public bool IsOutput => _allowedGoods.HasOutputGoods;

	public ReadOnlyList<StorableGoodAmount> AllowedGoods => _allowedGoods.Goods;

	public ReadOnlyHashSet<string> InputGoods => _allowedGoods.InputGoods;

	public ReadOnlyHashSet<string> OutputGoods => _allowedGoods.OutputGoods;

	public ReadOnlyList<GoodAmount> Stock => _storage.Goods;

	public bool IsUnblocked => _blockableObject.IsUnblocked;

	public event EventHandler<InventoryChangedEventArgs> InventoryChanged;

	public event EventHandler<InventoryStockChangedEventArgs> InventoryStockChanged;

	public event EventHandler<InventoryReservationChangedEventArgs> InventoryCapacityReservationChanged;

	public event EventHandler UnwantedStockDisappeared;

	public event EventHandler InventoryEnabled;

	public event EventHandler InventoryDisabled;

	public Inventory(GoodRegistryValueSerializer goodRegistryValueSerializer)
	{
		_goodRegistryValueSerializer = goodRegistryValueSerializer;
	}

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
		_districtInventoryAssigner = GetComponent<DistrictInventoryAssigner>();
	}

	public void Initialize(string componentName, int capacity, IEnumerable<StorableGoodAmount> storableGoodAmounts, bool publicInput, bool publicOutput, bool ignorableCapacity, IGoodDisallower goodDisallower)
	{
		_allowedGoods.Add(storableGoodAmounts);
		Capacity = capacity;
		ComponentName = componentName;
		PublicInput = publicInput;
		PublicOutput = publicOutput;
		_ignorableCapacity = ignorableCapacity;
		_goodDisallower = goodDisallower;
		_goodDisallower.DisallowedGoodsChanged += OnDisallowedGoodsChanged;
		DisableComponent();
	}

	public void Enable()
	{
		EnableComponent();
		_districtInventoryAssigner?.StartAssigningInventory(this);
		CheckIfUnwantedStockAppeared();
		foreach (GoodAmount good in _storage.Goods)
		{
			InvokeInventoryChangedEvent(good.GoodId);
		}
		InventoryEnabled?.Invoke(this, EventArgs.Empty);
	}

	public void Disable()
	{
		DisableComponent();
		_districtInventoryAssigner?.StopAssigningInventory(this);
		InventoryDisabled?.Invoke(this, EventArgs.Empty);
	}

	public void GiveProduced(GoodAmount good)
	{
		GiveInternal(good, StockChangeType.Produced);
	}

	public void GiveImported(GoodAmount good)
	{
		GiveInternal(good, StockChangeType.Imported);
	}

	public void GiveExisting(GoodAmount good)
	{
		GiveInternal(good, StockChangeType.None);
	}

	public void GiveExistingIgnoringCapacityReservation(GoodAmount good)
	{
		CheckCapacity(good);
		_storage.Add(good);
		InvokeInventoryStockChangedEvents(good, StockChangeType.None);
	}

	public void GiveExistingIgnoringCapacity(GoodAmount good)
	{
		_storage.Add(good);
		InvokeInventoryStockChangedEvents(good, StockChangeType.None);
	}

	public void ReserveCapacity(GoodAmount good)
	{
		CheckIfAllows(good);
		if (!HasUnreservedCapacity(good))
		{
			ThrowArgumentException($"Can't reserve capacity for {good}. Not enough unreserved capacity");
		}
		_reservedCapacity.Add(good);
		InvokeInventoryCapacityReservationChangedEvents(good);
	}

	public void UnreserveCapacity(GoodAmount good)
	{
		CheckIfAllows(good);
		if (_reservedCapacity.Amount(good.GoodId) < good.Amount)
		{
			ThrowArgumentException($"Can't unreserve capacity for {good}. It wasn't previously reserved.");
		}
		_reservedCapacity.Subtract(good);
		InvokeInventoryCapacityReservationChangedEvents(new GoodAmount(good.GoodId, -good.Amount));
	}

	public int ReservedCapacity(string good)
	{
		return _reservedCapacity.Amount(good);
	}

	public ReadOnlyList<GoodAmount> ReservedCapacity()
	{
		return _reservedCapacity.Goods;
	}

	public int UnreservedCapacity(string goodId)
	{
		int val = GoodCapacity(goodId) - _reservedCapacity.Amount(goodId);
		int val2 = Capacity - _storage.TotalAmount - _reservedCapacity.TotalAmount;
		return Math.Max(Math.Min(val, val2), 0);
	}

	public bool HasUnreservedCapacity(GoodAmount good)
	{
		return UnreservedCapacity(good.GoodId) >= good.Amount;
	}

	public bool HasUnreservedCapacity(string goodId)
	{
		if (Takes(goodId))
		{
			return UnreservedCapacity(goodId) > 0;
		}
		return false;
	}

	public void TakeConsumed(GoodAmount good)
	{
		TakeInternal(good, StockChangeType.Consumed);
	}

	public void TakeExported(GoodAmount good)
	{
		TakeInternal(good, StockChangeType.Exported);
	}

	public void TakeExisting(GoodAmount good)
	{
		TakeInternal(good, StockChangeType.None);
	}

	public void ReserveStock(GoodAmount good)
	{
		CheckIfHasStock(good, "reserve");
		_reservedStock.Add(good);
		InvokeInventoryChangedEvent(good.GoodId);
	}

	public bool HasUnreservedStock(GoodAmount good)
	{
		return UnreservedAmountInStock(good.GoodId) >= good.Amount;
	}

	public void UnreserveStock(GoodAmount good)
	{
		if (_reservedStock.Amount(good.GoodId) < good.Amount)
		{
			ThrowArgumentException($"Can't unreserve {good}. It wasn't previously reserved.");
		}
		_reservedStock.Subtract(good);
		InvokeInventoryChangedEvent(good.GoodId);
	}

	public bool HasUnreservedStock(string goodId)
	{
		if (Gives(goodId))
		{
			return UnreservedAmountInStock(goodId) > 0;
		}
		return false;
	}

	public int AmountInStock(string goodId)
	{
		return _storage.Amount(goodId);
	}

	public int UnreservedAmountInStock(string goodId)
	{
		int num = _storage.Amount(goodId);
		int num2 = _reservedStock.Amount(goodId);
		return num - num2;
	}

	public IEnumerable<GoodAmount> UnreservedStock()
	{
		foreach (GoodAmount good in _storage.Goods)
		{
			string goodId = good.GoodId;
			int amount = good.Amount;
			int num = _reservedStock.Amount(goodId);
			int num2 = amount - num;
			if (num2 > 0)
			{
				yield return new GoodAmount(goodId, num2);
			}
		}
	}

	public IEnumerable<GoodAmount> UnreservedTakeableStock()
	{
		foreach (GoodAmount item in UnreservedStock())
		{
			if (Gives(item.GoodId))
			{
				yield return item;
			}
		}
	}

	public IEnumerable<GoodAmount> UnreservedUnwantedStock()
	{
		foreach (GoodAmount item in UnreservedStock())
		{
			string goodId = item.GoodId;
			int num = LimitedAmount(goodId);
			int num2 = item.Amount - num;
			if (num2 > 0)
			{
				yield return new GoodAmount(goodId, num2);
			}
		}
	}

	public int UnwantedStockAmount()
	{
		int num = 0;
		foreach (GoodAmount good in _storage.Goods)
		{
			num += Math.Max(0, good.Amount - LimitedAmount(good.GoodId));
		}
		return num;
	}

	public int LimitedAmount(string goodId)
	{
		return Math.Min(_goodDisallower.AllowedAmount(goodId), _allowedGoods.GetAmount(goodId));
	}

	public bool Gives(string goodId)
	{
		return OutputGoods.Contains(goodId);
	}

	public bool Takes(string goodId)
	{
		return InputGoods.Contains(goodId);
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (!IsEmpty)
		{
			entitySaver.GetComponent(InventoryKey, ComponentName).Set(StorageKey, _storage, _goodRegistryValueSerializer);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		LoadFromNamedComponent(entityLoader, ComponentName);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Inventory " + base.Name + " " + ComponentName + ":");
		Guid entityId = GetComponent<EntityComponent>().EntityId;
		stringBuilder.AppendLine(string.Format("{0}: {1}", "entityId", entityId));
		stringBuilder.AppendLine($"enabled: {base.Enabled}");
		stringBuilder.AppendLine($"finished: {GetComponent<BlockObject>().IsFinished}");
		stringBuilder.AppendLine(string.Format("{0}: {1}", "Capacity", Capacity));
		stringBuilder.AppendLine(string.Format("{0}: {1}", "_allowedGoods", _allowedGoods));
		stringBuilder.AppendLine(string.Format("{0}: {1}", "_storage", _storage));
		stringBuilder.AppendLine(string.Format("{0}: {1}", "_reservedCapacity", _reservedCapacity));
		stringBuilder.AppendLine(string.Format("{0}: {1}", "_reservedStock", _reservedStock));
		return stringBuilder.ToString();
	}

	public void SetIgnorableCapacity(bool ignorableCapacity)
	{
		_ignorableCapacity = ignorableCapacity;
	}

	public void GetCapacity(List<GoodAmount> capacity)
	{
		if (_ignorableCapacity)
		{
			return;
		}
		foreach (StorableGoodAmount allowedGood in AllowedGoods)
		{
			string goodId = allowedGood.StorableGood.GoodId;
			int num = LimitedAmount(goodId);
			if (num > 0)
			{
				capacity.Add(new GoodAmount(goodId, num));
			}
		}
	}

	private void GiveInternal(GoodAmount good, StockChangeType stockChangeType)
	{
		CheckCapacity(good);
		CheckUnreservedCapacity(good);
		_storage.Add(good);
		InvokeInventoryStockChangedEvents(good, stockChangeType);
	}

	private void TakeInternal(GoodAmount good, StockChangeType stockChangeType)
	{
		CheckIfHasStock(good, "take");
		_storage.Subtract(good);
		CheckIfUnwantedStockDisappeared();
		InvokeInventoryStockChangedEvents(new GoodAmount(good.GoodId, -good.Amount), stockChangeType);
	}

	private void LoadFromNamedComponent(IEntityLoader entityLoader, string componentName)
	{
		if (entityLoader.TryGetComponent(InventoryKey, componentName, out var objectLoader))
		{
			_storage = objectLoader.Get(StorageKey, _goodRegistryValueSerializer);
		}
	}

	private void CheckUnreservedCapacity(GoodAmount good)
	{
		if (!HasUnreservedCapacity(good))
		{
			ThrowArgumentException($"Can't give {good}. There's not enough unreserved capacity.");
		}
	}

	private void CheckCapacity(GoodAmount good)
	{
		if (!HasCapacity(good))
		{
			ThrowArgumentException($"Can't give {good}. There's not enough capacity.");
		}
	}

	private bool HasStock(GoodAmount good)
	{
		return _storage.Amount(good.GoodId) >= good.Amount;
	}

	private bool Allows(string goodId)
	{
		return _allowedGoods.Contains(goodId);
	}

	private void CheckIfAllows(GoodAmount good)
	{
		string goodId = good.GoodId;
		if (!Allows(goodId))
		{
			ThrowArgumentException(goodId + " isn't allowed.");
		}
	}

	private void CheckIfHasStock(GoodAmount good, string action)
	{
		if (!HasStock(good))
		{
			ThrowArgumentException($"Can't {action} {good}. There's not enough stored.");
		}
		if (!HasUnreservedStock(good))
		{
			ThrowArgumentException($"Can't {action} {good}. There's not enough unreserved.");
		}
	}

	private bool HasCapacity(GoodAmount good)
	{
		int num = GoodCapacity(good.GoodId);
		int num2 = Capacity - _storage.TotalAmount;
		if (num >= good.Amount)
		{
			return num2 >= good.Amount;
		}
		return false;
	}

	private int GoodCapacity(string goodId)
	{
		return LimitedAmount(goodId) - _storage.Amount(goodId);
	}

	private void OnDisallowedGoodsChanged(object sender, DisallowedGoodsChangedEventArgs e)
	{
		CheckIfUnwantedStockAppeared();
		CheckIfUnwantedStockDisappeared();
		InvokeInventoryChangedEvent(e.GoodId);
	}

	private void CheckIfUnwantedStockAppeared()
	{
		if (base.Enabled && !HasUnwantedStock && UnwantedStockAmount() > 0)
		{
			HasUnwantedStock = true;
		}
	}

	private void CheckIfUnwantedStockDisappeared()
	{
		if (base.Enabled && HasUnwantedStock && UnwantedStockAmount() == 0)
		{
			HasUnwantedStock = false;
			UnwantedStockDisappeared?.Invoke(this, EventArgs.Empty);
		}
	}

	private void InvokeInventoryChangedEvent(string goodId)
	{
		InventoryChanged?.Invoke(this, new InventoryChangedEventArgs(goodId));
	}

	private void InvokeInventoryStockChangedEvents(GoodAmount goodAmount, StockChangeType stockChangeType)
	{
		InvokeInventoryChangedEvent(goodAmount.GoodId);
		InventoryStockChanged?.Invoke(this, new InventoryStockChangedEventArgs(goodAmount, stockChangeType));
	}

	private void InvokeInventoryCapacityReservationChangedEvents(GoodAmount goodAmount)
	{
		InvokeInventoryChangedEvent(goodAmount.GoodId);
		InventoryCapacityReservationChanged?.Invoke(this, new InventoryReservationChangedEventArgs(goodAmount));
	}

	private void ThrowArgumentException(string customMessage)
	{
		throw new ArgumentException(customMessage + "\n" + ToString());
	}
}
