using Timberborn.SingletonSystem;

namespace Timberborn.TickSystem;

[Singleton]
public interface ITickableSingleton
{
	void Tick();
}
