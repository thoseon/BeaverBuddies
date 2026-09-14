using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.SingletonSystem;

namespace Timberborn.Goods;

internal class GoodService : IGoodService, ILoadableSingleton
{
	private readonly ISpecService _specService;

	private readonly IGoodFilter _goodFilter;

	private readonly List<string> _goods = new List<string>();

	private readonly Dictionary<string, GoodSpec> _goodSpecsById = new Dictionary<string, GoodSpec>();

	public ReadOnlyList<string> Goods => _goods.AsReadOnlyList();

	public GoodService(ISpecService specService, IGoodFilter goodFilter)
	{
		_specService = specService;
		_goodFilter = goodFilter;
	}

	public void Load()
	{
		LoadGoods();
	}

	public bool HasGood(string id)
	{
		return _goodSpecsById.ContainsKey(id);
	}

	public GoodSpec GetGoodOrNull(string id)
	{
		return _goodSpecsById.GetValueOrDefault(id);
	}

	public GoodSpec GetGood(string id)
	{
		if (_goodSpecsById.TryGetValue(id, out var value))
		{
			return value;
		}
		throw new ArgumentException("GoodSpec with id " + id + " not found!");
	}

	public IEnumerable<string> GetGoodsForGroup(string groupId)
	{
		foreach (string good in Goods)
		{
			if (GetGood(good).GoodGroupId == groupId)
			{
				yield return good;
			}
		}
	}

	public IEnumerable<string> GetGoodsForType(string goodType)
	{
		foreach (string good in Goods)
		{
			if (GetGood(good).GoodType == goodType)
			{
				yield return good;
			}
		}
	}

	private void LoadGoods()
	{
		foreach (GoodSpec item in from goodSpec in _specService.GetSpecs<GoodSpec>()
			orderby goodSpec.GoodOrder
			select goodSpec)
		{
			if (item.Blueprint.IsAllowedByFeatureToggles() && _goodFilter.IsUsable(item))
			{
				AddGood(item);
			}
		}
		foreach (GoodSpec item2 in _goodSpecsById.Values.ToList())
		{
			BindGoodToItsBackwardCompatibleIds(item2);
		}
	}

	private void AddGood(GoodSpec goodSpec)
	{
		_goods.Add(goodSpec.Id);
		_goodSpecsById[goodSpec.Id] = goodSpec;
	}

	private void BindGoodToItsBackwardCompatibleIds(GoodSpec goodSpec)
	{
		foreach (string backwardCompatibleId in goodSpec.BackwardCompatibleIds)
		{
			_goodSpecsById.TryAdd(backwardCompatibleId, goodSpec);
		}
	}
}
