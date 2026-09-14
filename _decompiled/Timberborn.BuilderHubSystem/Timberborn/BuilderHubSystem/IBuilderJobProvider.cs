using Timberborn.BehaviorSystem;
using Timberborn.Navigation;
using Timberborn.PrioritySystem;

namespace Timberborn.BuilderHubSystem;

public interface IBuilderJobProvider
{
	int ProviderPriority { get; }

	(Behavior, Decision) GetJob(Accessible start, BehaviorAgent agent, Priority priority);
}
