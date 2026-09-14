using System;
using System.Collections.Generic;

namespace Timberborn.Goods;

public class GoodAmountComparer : IComparer<GoodAmount>
{
	public int Compare(GoodAmount x, GoodAmount y)
	{
		int num = x.Amount.CompareTo(y.Amount);
		if (num == 0)
		{
			return string.Compare(x.GoodId, y.GoodId, StringComparison.Ordinal);
		}
		return num;
	}
}
