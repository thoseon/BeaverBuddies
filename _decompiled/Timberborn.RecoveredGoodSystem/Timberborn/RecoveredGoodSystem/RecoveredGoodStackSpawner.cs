using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Goods;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.RecoveredGoodSystem;

internal class RecoveredGoodStackSpawner : IUpdatableSingleton
{
	private readonly IBlockService _blockService;

	private readonly RecoveredGoodStackCoordinatesFinder _recoveredGoodStackCoordinatesFinder;

	private readonly RecoveredGoodStackFactory _recoveredGoodStackFactory;

	private readonly Dictionary<Vector3Int, List<GoodAmount>> _awaitingGoods = new Dictionary<Vector3Int, List<GoodAmount>>();

	private readonly Dictionary<Vector3Int, List<GoodAmount>> _validatedGoods = new Dictionary<Vector3Int, List<GoodAmount>>();

	public RecoveredGoodStackSpawner(IBlockService blockService, RecoveredGoodStackCoordinatesFinder recoveredGoodStackCoordinatesFinder, RecoveredGoodStackFactory recoveredGoodStackFactory)
	{
		_blockService = blockService;
		_recoveredGoodStackCoordinatesFinder = recoveredGoodStackCoordinatesFinder;
		_recoveredGoodStackFactory = recoveredGoodStackFactory;
	}

	public void AddAwaitingGoods(Vector3Int position, IEnumerable<GoodAmount> recoveredGoods)
	{
		_awaitingGoods.GetOrAdd(position).AddRange(recoveredGoods);
	}

	public void UpdateSingleton()
	{
		if (_awaitingGoods.Count > 0)
		{
			ValidateAwaitingGoods();
			SpawnValidatedGoods();
		}
	}

	private void ValidateAwaitingGoods()
	{
		foreach (KeyValuePair<Vector3Int, List<GoodAmount>> awaitingGood in _awaitingGoods)
		{
			ValidateAwaitingGood(awaitingGood.Key, awaitingGood.Value);
		}
		_awaitingGoods.Clear();
	}

	private void ValidateAwaitingGood(Vector3Int coordinates, IReadOnlyCollection<GoodAmount> awaitingGood)
	{
		if (_recoveredGoodStackCoordinatesFinder.FindValidCoordinates(coordinates, out var validCoordinates) && !TryMergeAwaitingGood(validCoordinates, awaitingGood))
		{
			_validatedGoods.GetOrAdd(validCoordinates).AddRange(awaitingGood);
		}
	}

	private bool TryMergeAwaitingGood(Vector3Int coordinate, IEnumerable<GoodAmount> awaitingGood)
	{
		RecoveredGoodStack recoveredGoodStack = _blockService.GetObjectsWithComponentAt<RecoveredGoodStack>(coordinate).FirstOrDefault();
		if ((bool)recoveredGoodStack)
		{
			recoveredGoodStack.GiveGoodAmounts(awaitingGood);
			return true;
		}
		return false;
	}

	private void SpawnValidatedGoods()
	{
		foreach (KeyValuePair<Vector3Int, List<GoodAmount>> validatedGood in _validatedGoods)
		{
			_recoveredGoodStackFactory.Create(validatedGood.Key, validatedGood.Value);
		}
		_validatedGoods.Clear();
	}
}
