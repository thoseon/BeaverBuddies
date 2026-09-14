using System;
using System.Collections.Generic;
using Timberborn.BlockObjectModelSystem;
using Timberborn.Common;
using Timberborn.MapStateSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.ModelHiding;

internal class HidableModels : ILoadableSingleton
{
	private readonly MapSize _mapSize;

	private readonly List<HashSet<BlockObjectModelController>> _models = new List<HashSet<BlockObjectModelController>>();

	public HidableModels(MapSize mapSize)
	{
		_mapSize = mapSize;
	}

	public void Load()
	{
		for (int i = 0; i <= _mapSize.TotalSize.z; i++)
		{
			_models.Add(new HashSet<BlockObjectModelController>());
		}
	}

	public void Add(BlockObjectModelController model)
	{
		Remove(model);
		(int, int) modelRange = GetModelRange(model);
		int item = modelRange.Item1;
		int item2 = modelRange.Item2;
		for (int i = item; i < item2; i++)
		{
			_models[i].Add(model);
		}
	}

	public void Remove(BlockObjectModelController model)
	{
		(int, int) modelRange = GetModelRange(model);
		int item = modelRange.Item1;
		int item2 = modelRange.Item2;
		for (int i = item; i < item2; i++)
		{
			_models[i].Remove(model);
		}
	}

	public ReadOnlyHashSet<BlockObjectModelController> ModelsAt(int level)
	{
		return _models[level].AsReadOnlyHashSet();
	}

	public bool TryGetHidableRange(out int maxLevel)
	{
		maxLevel = int.MinValue;
		for (int i = 0; i < _models.Count; i++)
		{
			if (!_models[i].IsEmpty())
			{
				maxLevel = Math.Max(i, maxLevel);
			}
		}
		return maxLevel != int.MinValue;
	}

	private static (int, int) GetModelRange(BlockObjectModelController model)
	{
		int item = (model.HasUndergroundModel ? model.UndergroundBaseZ : model.BlockObject.GetBaseLevel());
		int topLevel = model.BlockObject.GetTopLevel();
		return (item, topLevel);
	}
}
