using Timberborn.BaseComponentSystem;
using Timberborn.LifeSystem;

namespace Timberborn.Bots;

internal class BotLongevity : BaseComponent, ILongevity
{
	public float ExpectedLongevity => 1f;
}
