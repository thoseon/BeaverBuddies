using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using UnityEngine;

namespace Timberborn.RecoverableGoodSystem;

public class RecoverableGoodProvider : BaseComponent, IAwakableComponent
{
	private Inventories _inventories;

	private readonly List<IRecoverableGoodMultiplier> _recoverableGoodMultipliers = new List<IRecoverableGoodMultiplier>();

	private bool _recoveryDisabled;

	public void Awake()
	{
		_inventories = GetComponent<Inventories>();
		GetComponents(_recoverableGoodMultipliers);
	}

	public void DisableGoodRecovery()
	{
		_recoveryDisabled = true;
	}

	public void EnableGoodRecovery()
	{
		_recoveryDisabled = false;
	}

	public void GetRecoverableGoods(RecoverableGoodRegistry recoverableGoodRegistry)
	{
		if (!_recoveryDisabled)
		{
			for (int i = 0; i < _inventories.AllInventories.Count; i++)
			{
				Inventory inventory = _inventories.AllInventories[i];
				AddGoodsFromInventory(recoverableGoodRegistry, inventory);
			}
		}
	}

	private void AddGoodsFromInventory(RecoverableGoodRegistry recoverableGoodRegistry, Inventory inventory)
	{
		float totalMultiplierForInventory = GetTotalMultiplierForInventory(inventory);
		foreach (GoodAmount item in inventory.Stock)
		{
			recoverableGoodRegistry.Add(GetMultipliedAmount(item, totalMultiplierForInventory));
		}
	}

	private float GetTotalMultiplierForInventory(Inventory inventory)
	{
		float num = 1f;
		for (int i = 0; i < _recoverableGoodMultipliers.Count; i++)
		{
			num *= _recoverableGoodMultipliers[i].GetMultiplierForInventory(inventory);
		}
		return num;
	}

	private static GoodAmount GetMultipliedAmount(GoodAmount goodAmount, float multiplier)
	{
		float f = (float)goodAmount.Amount * multiplier;
		return new GoodAmount(goodAmount.GoodId, Mathf.CeilToInt(f));
	}
}
