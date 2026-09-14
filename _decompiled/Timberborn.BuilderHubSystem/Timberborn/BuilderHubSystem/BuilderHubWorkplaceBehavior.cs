using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BehaviorSystem;
using Timberborn.Buildings;
using Timberborn.EntitySystem;
using Timberborn.Navigation;
using Timberborn.PrioritySystem;
using Timberborn.WorkSystem;

namespace Timberborn.BuilderHubSystem;

public class BuilderHubWorkplaceBehavior : WorkplaceBehavior, IInitializableEntity
{
	private readonly ImmutableArray<IBuilderJobProvider> _providers;

	private Accessible _accessible;

	public BuilderHubWorkplaceBehavior(IEnumerable<IBuilderJobProvider> builderJobProviders)
	{
		_providers = builderJobProviders.OrderBy((IBuilderJobProvider provider) => provider.ProviderPriority).ToImmutableArray();
	}

	public void InitializeEntity()
	{
		_accessible = GetComponent<BuildingAccessible>().Accessible;
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		foreach (Priority item in Priorities.Descending)
		{
			foreach (IBuilderJobProvider provider in _providers)
			{
				var (behavior, decision) = provider.GetJob(_accessible, agent, item);
				if (!decision.ShouldReleaseNow)
				{
					return Decision.TransferNow(behavior, in decision);
				}
			}
		}
		return Decision.ReleaseNow();
	}
}
