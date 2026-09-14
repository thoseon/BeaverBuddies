using Timberborn.Goods;
using Timberborn.Yielding;

namespace Timberborn.YielderFinding;

public readonly struct YielderSearchResult
{
	public Yielder Yielder { get; }

	public GoodAmount Yield { get; }

	public bool NoYielderInRange { get; }

	public bool HasYielder => Yielder;

	private YielderSearchResult(Yielder yielder, GoodAmount yield, bool noYielderInRange)
	{
		Yielder = yielder;
		Yield = yield;
		NoYielderInRange = noYielderInRange;
	}

	public static YielderSearchResult CreateSearchResult(Yielder yielder, GoodAmount yield)
	{
		return new YielderSearchResult(yielder, yield, noYielderInRange: false);
	}

	public static YielderSearchResult CreateNoYielderInRange()
	{
		return new YielderSearchResult(null, default(GoodAmount), noYielderInRange: true);
	}

	public static YielderSearchResult CreateEmpty()
	{
		return new YielderSearchResult(null, default(GoodAmount), noYielderInRange: false);
	}
}
