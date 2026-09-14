using Timberborn.BaseComponentSystem;
using Timberborn.Yielding;

namespace Timberborn.Fields;

internal class FarmHouseYielderRetriever : BaseComponent, IYielderRetriever
{
	public bool TryGetYielder(BaseComponent component, out Yielder yielder)
	{
		Crop component2 = component.GetComponent<Crop>();
		if (component2 != null)
		{
			yielder = component2.Yielder;
			return true;
		}
		yielder = null;
		return false;
	}
}
