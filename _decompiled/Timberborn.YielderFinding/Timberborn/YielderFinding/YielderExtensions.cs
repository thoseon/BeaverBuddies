using Timberborn.NaturalResourcesLifecycle;
using Timberborn.Yielding;

namespace Timberborn.YielderFinding;

public static class YielderExtensions
{
	public static bool IsYieldingOrAlive(this Yielder yielder)
	{
		if (!yielder.IsYielding)
		{
			return yielder.IsAlive();
		}
		return true;
	}

	public static bool IsAlive(this Yielder yielder)
	{
		return !yielder.GetComponent<LivingNaturalResource>().IsDead;
	}
}
