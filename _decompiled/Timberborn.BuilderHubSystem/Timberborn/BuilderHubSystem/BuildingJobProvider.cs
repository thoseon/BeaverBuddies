using Timberborn.BehaviorSystem;
using Timberborn.ConstructionSites;
using Timberborn.Navigation;
using Timberborn.PrioritySystem;

namespace Timberborn.BuilderHubSystem;

internal class BuildingJobProvider : IBuilderJobProvider
{
	private readonly ConstructionRegistry _constructionRegistry;

	public int ProviderPriority => 1;

	public BuildingJobProvider(ConstructionRegistry constructionRegistry)
	{
		_constructionRegistry = constructionRegistry;
	}

	public (Behavior, Decision) GetJob(Accessible start, BehaviorAgent agent, Priority priority)
	{
		foreach (ConstructionJob job in _constructionRegistry.GetJobs(priority))
		{
			var (item, item2) = job.StartConstructionJob(agent, start);
			if (!item2.ShouldReleaseNow)
			{
				return (item, item2);
			}
		}
		return (null, Decision.ReleaseNow());
	}
}
