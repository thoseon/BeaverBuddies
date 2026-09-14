using Timberborn.BaseComponentSystem;

namespace Timberborn.Explosions;

public class MortalDiedFromExplosionEvent
{
	public BaseComponent Source { get; }

	public MortalDiedFromExplosionEvent(BaseComponent source)
	{
		Source = source;
	}
}
