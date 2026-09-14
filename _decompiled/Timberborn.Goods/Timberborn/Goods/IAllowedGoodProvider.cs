using System.Collections.Generic;

namespace Timberborn.Goods;

public interface IAllowedGoodProvider
{
	IEnumerable<string> GetAllowedGoods();
}
