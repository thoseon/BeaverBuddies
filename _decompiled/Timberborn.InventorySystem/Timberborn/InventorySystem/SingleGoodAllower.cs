using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.DuplicationSystem;
using Timberborn.Goods;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.InventorySystem;

public class SingleGoodAllower : BaseComponent, IAwakableComponent, IFinishedStateListener, IPersistentEntity, IInitializableGoodDisallower, IGoodDisallower, IDuplicable<SingleGoodAllower>, IDuplicable
{
	private static readonly ComponentKey SingleGoodAllowerKey = new ComponentKey("SingleGoodAllower");

	private static readonly PropertyKey<SerializedGood> AllowedGoodKey = new PropertyKey<SerializedGood>("AllowedGood");

	private readonly SerializedGoodValueSerializer _serializedGoodValueSerializer;

	private Inventory _inventory;

	public string AllowedGood { get; private set; }

	public bool HasAllowedGood => AllowedGood != null;

	public event EventHandler<DisallowedGoodsChangedEventArgs> DisallowedGoodsChanged;

	public SingleGoodAllower(SerializedGoodValueSerializer serializedGoodValueSerializer)
	{
		_serializedGoodValueSerializer = serializedGoodValueSerializer;
	}

	public void Awake()
	{
		DisableComponent();
	}

	public void Initialize(Inventory inventory)
	{
		_inventory = inventory;
	}

	public void Allow(string goodId)
	{
		Disallow();
		AllowedGood = goodId;
		if (goodId != null)
		{
			InvokeDisallowedGoodsChangedEvent(goodId);
		}
	}

	public void Disallow()
	{
		string allowedGood = AllowedGood;
		AllowedGood = null;
		if (allowedGood != null)
		{
			InvokeDisallowedGoodsChangedEvent(allowedGood);
		}
	}

	public int AllowedAmount(string goodId)
	{
		if (!(AllowedGood == goodId) || HasOtherGoods())
		{
			return 0;
		}
		return int.MaxValue;
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (HasAllowedGood)
		{
			entitySaver.GetComponent(SingleGoodAllowerKey).Set(AllowedGoodKey, new SerializedGood(AllowedGood), _serializedGoodValueSerializer);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(SingleGoodAllowerKey, out var objectLoader) && objectLoader.GetObsoletable(AllowedGoodKey, _serializedGoodValueSerializer, out var value) && _inventory.Takes(value.Id))
		{
			AllowedGood = value.Id;
		}
	}

	public void DuplicateFrom(SingleGoodAllower source)
	{
		string allowedGood = source.AllowedGood;
		if (allowedGood == null || _inventory.Takes(allowedGood))
		{
			Allow(allowedGood);
		}
	}

	private bool HasOtherGoods()
	{
		foreach (GoodAmount item in _inventory.Stock)
		{
			if (item.GoodId != AllowedGood)
			{
				return true;
			}
		}
		return false;
	}

	private void InvokeDisallowedGoodsChangedEvent(string goodId)
	{
		DisallowedGoodsChanged?.Invoke(this, new DisallowedGoodsChangedEventArgs(goodId));
	}
}
