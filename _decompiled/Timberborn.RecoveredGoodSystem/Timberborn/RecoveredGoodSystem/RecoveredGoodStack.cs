using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.GoodStackSystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.TerrainPhysics;
using UnityEngine;

namespace Timberborn.RecoveredGoodSystem;

public class RecoveredGoodStack : BaseComponent, IGoodStackInventory, IInitializableEntity, INonStackPickable
{
	private readonly EntityService _entityService;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	public Inventory Inventory { get; private set; }

	public RecoveredGoodStack(EntityService entityService, IRandomNumberGenerator randomNumberGenerator)
	{
		_entityService = entityService;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void InitializeInventory(Inventory inventory)
	{
		Asserts.FieldIsNull(this, Inventory, "Inventory");
		Inventory = inventory;
		Inventory.Enable();
		Inventory.InventoryChanged += OnInventoryChanged;
	}

	public void InitializeEntity()
	{
		if (TryGetComponent<RecoveredGoodStackInit>(out var component))
		{
			GiveGoodAmounts(component.GoodAmounts);
			int rotation = _randomNumberGenerator.Range(0, 360);
			GetComponent<RecoveredGoodStackModel>().SetRotation(rotation);
		}
		if (Inventory.IsEmpty)
		{
			Debug.LogWarning($"RecoveredGoodStack at {GetComponent<BlockObject>().Coordinates} " + "was empty after initialization. Deleting.");
			Delete();
		}
	}

	public void MergeInto(RecoveredGoodStack otherGoodStack)
	{
		otherGoodStack.GiveGoodAmounts(Inventory.Stock);
		Delete();
	}

	public void GiveGoodAmounts(IEnumerable<GoodAmount> goodAmounts)
	{
		foreach (GoodAmount goodAmount in goodAmounts)
		{
			Inventory.GiveProduced(goodAmount);
		}
	}

	public void Delete()
	{
		_entityService.Delete(this);
	}

	private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
	{
		if (Inventory.IsEmpty)
		{
			Delete();
		}
	}
}
