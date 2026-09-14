using Timberborn.BaseComponentSystem;
using Timberborn.Yielding;

namespace Timberborn.Gathering;

internal class GathererFlagYielderRetriever : BaseComponent, IYielderRetriever
{
	public bool TryGetYielder(BaseComponent component, out Yielder yielder)
	{
		Gatherable component2 = component.GetComponent<Gatherable>();
		if (component2 != null)
		{
			yielder = component2.Yielder;
			return true;
		}
		yielder = null;
		return false;
	}
}
