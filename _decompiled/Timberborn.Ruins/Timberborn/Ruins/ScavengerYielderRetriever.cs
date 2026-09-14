using Timberborn.BaseComponentSystem;
using Timberborn.Yielding;

namespace Timberborn.Ruins;

internal class ScavengerYielderRetriever : BaseComponent, IYielderRetriever
{
	public bool TryGetYielder(BaseComponent component, out Yielder yielder)
	{
		Ruin component2 = component.GetComponent<Ruin>();
		if (component2 != null)
		{
			yielder = component2.Yielder;
			return true;
		}
		yielder = null;
		return false;
	}
}
