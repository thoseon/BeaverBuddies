using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.GoodCollectionSystem;
using Timberborn.Goods;
using Timberborn.SingletonSystem;

namespace Timberborn.GameGoods;

internal class GameGoodFilter : IGoodFilter, ILoadableSingleton
{
	private readonly ISpecService _specService;

	private readonly ImmutableArray<IGoodCollectionIdsProvider> _goodCollectionIdsProviders;

	private readonly HashSet<string> _allowedGoods = new HashSet<string>();

	public GameGoodFilter(ISpecService specService, IEnumerable<IGoodCollectionIdsProvider> goodCollectionIdsProviders)
	{
		_specService = specService;
		_goodCollectionIdsProviders = goodCollectionIdsProviders.ToImmutableArray();
	}

	public void Load()
	{
		HashSet<string> hashSet = _goodCollectionIdsProviders.SelectMany((IGoodCollectionIdsProvider provider) => provider.GetGoodCollectionIds()).ToHashSet();
		foreach (GoodCollectionSpec spec in _specService.GetSpecs<GoodCollectionSpec>())
		{
			if (hashSet.Contains(spec.CollectionId))
			{
				_allowedGoods.AddRange(spec.Goods);
			}
		}
		List<GoodSpec> source = _specService.GetSpecs<GoodSpec>().ToList();
		foreach (string goodId in _allowedGoods)
		{
			if (source.All((GoodSpec good) => good.Id != goodId))
			{
				throw new ArgumentException("There is no GoodSpec with id: " + goodId);
			}
		}
	}

	public bool IsUsable(GoodSpec goodSpec)
	{
		return _allowedGoods.Contains(goodSpec.Id);
	}
}
