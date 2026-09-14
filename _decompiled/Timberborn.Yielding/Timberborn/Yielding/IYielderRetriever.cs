using Timberborn.BaseComponentSystem;

namespace Timberborn.Yielding;

public interface IYielderRetriever
{
	bool TryGetYielder(BaseComponent component, out Yielder yielder);
}
