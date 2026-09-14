using System;
using System.Collections.Generic;
using Timberborn.DeconstructionSystem;
using Timberborn.Goods;
using Timberborn.InputSystem;
using Timberborn.RecoverableGoodSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.RecoveredGoodSystem;

internal class BuildingGoodsRecoveryService : ILoadableSingleton
{
	private static readonly string DontRecoverGoodsKey = "DontRecoverGoods";

	private readonly EventBus _eventBus;

	private readonly InputService _inputService;

	private readonly RecoveredGoodStackSpawner _recoveredGoodStackSpawner;

	private readonly RecoverableGoodRegistry _recoverableGoodRegistry = new RecoverableGoodRegistry();

	private readonly List<GoodAmount> _recoveredGoods = new List<GoodAmount>();

	public BuildingGoodsRecoveryService(EventBus eventBus, InputService inputService, RecoveredGoodStackSpawner recoveredGoodStackSpawner)
	{
		_eventBus = eventBus;
		_inputService = inputService;
		_recoveredGoodStackSpawner = recoveredGoodStackSpawner;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnBuildingDeconstructed(BuildingDeconstructedEvent buildingDeconstructedEvent)
	{
		if (!_inputService.IsKeyHeld(DontRecoverGoodsKey))
		{
			PrepareToSpawning(buildingDeconstructedEvent.Deconstructible, buildingDeconstructedEvent.Coordinates);
		}
	}

	private void PrepareToSpawning(Deconstructible deconstructible, IReadOnlyList<Vector3Int> coordinates)
	{
		if (coordinates.Count > 0)
		{
			deconstructible.GetComponent<RecoverableGoodProvider>().GetRecoverableGoods(_recoverableGoodRegistry);
			if (_recoverableGoodRegistry.TotalAmount > 0)
			{
				SplitGoodsAndAddToSpawnQueue(coordinates);
				CheckIfAllGoodsWereRecovered(deconstructible);
			}
		}
	}

	private void SplitGoodsAndAddToSpawnQueue(IReadOnlyList<Vector3Int> coordinates)
	{
		for (int i = 0; i < coordinates.Count; i++)
		{
			_recoverableGoodRegistry.TakePercent(1f / (float)(coordinates.Count - i), _recoveredGoods);
			if (_recoveredGoods.Count > 0)
			{
				_recoveredGoodStackSpawner.AddAwaitingGoods(coordinates[i], _recoveredGoods);
				_recoveredGoods.Clear();
			}
		}
	}

	private void CheckIfAllGoodsWereRecovered(Deconstructible deconstructible)
	{
		if (_recoverableGoodRegistry.TotalAmount > 0 || _recoverableGoodRegistry.GoodAmounts.Count > 0)
		{
			throw new InvalidOperationException($"Not all goods were recovered from {deconstructible}: " + string.Join(", ", _recoverableGoodRegistry.GoodAmounts));
		}
	}
}
