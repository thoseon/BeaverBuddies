using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.Goods;

public interface IGoodService
{
	ReadOnlyList<string> Goods { get; }

	bool HasGood(string id);

	GoodSpec GetGoodOrNull(string id);

	GoodSpec GetGood(string id);

	IEnumerable<string> GetGoodsForGroup(string groupId);

	IEnumerable<string> GetGoodsForType(string goodType);
}
