using Timberborn.SingletonSystem;

namespace Timberborn.Automation;

[Singleton]
public interface ICommittingSingleton
{
	void CommitTick();
}
