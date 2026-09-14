using Timberborn.BehaviorSystem;
using Timberborn.BuilderHubSystem;
using Timberborn.Navigation;
using Timberborn.PrioritySystem;

namespace Timberborn.RecoveredGoodSystem;

internal class RecoverGoodStackJobProvider : IBuilderJobProvider
{
	private readonly PrioritizedRecoveredGoodStackRegistry _prioritizedRecoveredGoodStackRegistry;

	public int ProviderPriority => 3;

	public RecoverGoodStackJobProvider(PrioritizedRecoveredGoodStackRegistry prioritizedRecoveredGoodStackRegistry)
	{
		_prioritizedRecoveredGoodStackRegistry = prioritizedRecoveredGoodStackRegistry;
	}

	public (Behavior, Decision) GetJob(Accessible start, BehaviorAgent agent, Priority priority)
	{
		RecoveredGoodStackCarryingBehavior component = agent.GetComponent<RecoveredGoodStackCarryingBehavior>();
		foreach (RecoveredGoodStack recoveredGoodStack in _prioritizedRecoveredGoodStackRegistry.GetRecoveredGoodStacks(priority))
		{
			if (IsStackRecoverable(recoveredGoodStack, start))
			{
				Decision item = component.FindInventoryAndStartCarrying(recoveredGoodStack);
				if (!item.ShouldReleaseNow)
				{
					return (component, item);
				}
			}
		}
		return (null, Decision.ReleaseNow());
	}

	private static bool IsStackRecoverable(RecoveredGoodStack recoveredGoodStack, Accessible start)
	{
		if (recoveredGoodStack.Inventory.HasAnyUnreservedStock)
		{
			Accessible enabledComponent = recoveredGoodStack.GetEnabledComponent<Accessible>();
			if (enabledComponent != null)
			{
				return start.IsReachableByRoadToTerrain(enabledComponent);
			}
		}
		return false;
	}
}
