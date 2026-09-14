using System;
using System.Collections.Generic;
using Timberborn.Buildings;
using Timberborn.GameSceneLoading;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.NewGameConfigurationSystem;
using Timberborn.SceneLoading;
using Timberborn.SimpleOutputBuildings;

namespace Timberborn.GameStartup;

public class StartingGoodsProvider
{
	private readonly ISceneLoader _sceneLoader;

	public StartingGoodsProvider(ISceneLoader sceneLoader)
	{
		_sceneLoader = sceneLoader;
	}

	public void AddStartingInventory(Building startingBuilding)
	{
		ModifyStartingInventory(startingBuilding, delegate(Inventory inventory, GoodAmount initialGood)
		{
			inventory.GiveExistingIgnoringCapacity(initialGood);
		});
	}

	public void RemoveStartingInventory(Building startingBuilding)
	{
		ModifyStartingInventory(startingBuilding, delegate(Inventory inventory, GoodAmount initialGood)
		{
			inventory.TakeExisting(initialGood);
		});
	}

	private void ModifyStartingInventory(Building startingBuilding, Action<Inventory, GoodAmount> modifyAction)
	{
		if (startingBuilding == null)
		{
			return;
		}
		Inventory inventory = startingBuilding.GetComponent<SimpleOutputInventory>().Inventory;
		foreach (GoodAmount item in InitialGoods())
		{
			modifyAction(inventory, item);
		}
	}

	private IEnumerable<GoodAmount> InitialGoods()
	{
		GameModeSpec gameMode = _sceneLoader.GetSceneParameters<GameSceneParameters>().NewGameConfiguration.GameMode;
		yield return new GoodAmount("Berries", gameMode.StartingFood);
		yield return new GoodAmount("Water", gameMode.StartingWater);
	}
}
