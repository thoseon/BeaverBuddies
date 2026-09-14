using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.FactionSystem;
using Timberborn.GoodCollectionSystem;
using Timberborn.Goods;
using Timberborn.GoodsUI;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.MapEditorStockpilesUI;

internal class FixedStockpileGoodProvider : ILoadableSingleton
{
	private readonly IGoodService _goodService;

	private readonly ISpecService _specService;

	private readonly GoodDescriber _goodDescriber;

	private readonly FactionSpecService _factionSpecService;

	private readonly CommonGoodCollectionIdsProvider _commonGoodCollectionIdsProvider;

	private readonly Dictionary<string, HashSet<string>> _collectionIdToGoods = new Dictionary<string, HashSet<string>>();

	private string _commonId;

	public FixedStockpileGoodProvider(IGoodService goodService, ISpecService specService, GoodDescriber goodDescriber, FactionSpecService factionSpecService, CommonGoodCollectionIdsProvider commonGoodCollectionIdsProvider)
	{
		_goodService = goodService;
		_specService = specService;
		_goodDescriber = goodDescriber;
		_factionSpecService = factionSpecService;
		_commonGoodCollectionIdsProvider = commonGoodCollectionIdsProvider;
	}

	public void Load()
	{
		foreach (GoodCollectionSpec spec in _specService.GetSpecs<GoodCollectionSpec>())
		{
			_collectionIdToGoods.GetOrAdd(spec.CollectionId).AddRange(spec.Goods);
		}
		_commonId = _commonGoodCollectionIdsProvider.GetGoodCollectionIds().Single();
	}

	public ImmutableArray<string> GetGoods(string goodType)
	{
		return (from good in _goodService.GetGoodsForType(goodType)
			select _goodService.GetGood(good) into good
			orderby GetGoodOrder(good.Id), good.PluralDisplayName.Value
			select good.Id).ToImmutableArray();
	}

	public string GetDisplayText(string goodId)
	{
		string value = _goodService.GetGood(goodId).PluralDisplayName.Value;
		if (IsCommonGood(goodId))
		{
			return value;
		}
		if (!IsSingleFactionGood(goodId, out var singleFactionId))
		{
			return "✱ - " + value;
		}
		return _factionSpecService.GetFaction(singleFactionId).DisplayName.Value + " - " + value;
	}

	public Sprite GetIcon(string goodId)
	{
		return _goodDescriber.GetIcon(goodId);
	}

	public string GetTooltip(string goodId)
	{
		IEnumerable<string> values = from faction in _factionSpecService.Factions
			where IsFactionGood(faction, goodId)
			select faction.DisplayName.Value;
		return string.Join(", ", values);
	}

	private int GetGoodOrder(string goodId)
	{
		if (IsCommonGood(goodId))
		{
			return int.MinValue;
		}
		if (!IsSingleFactionGood(goodId, out var singleFactionId))
		{
			return -2147483647;
		}
		return _factionSpecService.GetFaction(singleFactionId).Order;
	}

	private bool IsCommonGood(string goodId)
	{
		return _collectionIdToGoods[_commonId].Contains(goodId);
	}

	private bool IsSingleFactionGood(string goodId, out string singleFactionId)
	{
		int num = 0;
		singleFactionId = string.Empty;
		foreach (FactionSpec faction in _factionSpecService.Factions)
		{
			if (IsFactionGood(faction, goodId))
			{
				singleFactionId = faction.Id;
				num++;
			}
		}
		return num == 1;
	}

	private bool IsFactionGood(FactionSpec factionSpec, string goodId)
	{
		foreach (string goodCollectionId in factionSpec.GoodCollectionIds)
		{
			if (_collectionIdToGoods[goodCollectionId].Contains(goodId))
			{
				return true;
			}
		}
		return false;
	}
}
