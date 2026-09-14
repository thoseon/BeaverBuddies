using Timberborn.BehaviorSystem;
using Timberborn.BuilderHubSystem;
using Timberborn.Navigation;
using Timberborn.PrioritySystem;

namespace Timberborn.Demolishing;

internal class DemolishJobProvider : IBuilderJobProvider
{
	private readonly DemolishJobs _demolishJobs;

	public int ProviderPriority => 2;

	public DemolishJobProvider(DemolishJobs demolishJobs)
	{
		_demolishJobs = demolishJobs;
	}

	public (Behavior, Decision) GetJob(Accessible start, BehaviorAgent agent, Priority priority)
	{
		Demolisher component = agent.GetComponent<Demolisher>();
		DemolishJob demolishJob = null;
		float num = float.MaxValue;
		foreach (DemolishJob job in _demolishJobs.GetJobs(priority))
		{
			if (job.CanStartJob(component) && IsReachable(job, start, out var distance) && distance < num)
			{
				demolishJob = job;
				num = distance;
			}
		}
		if ((bool)demolishJob)
		{
			var (item, item2) = demolishJob.StartBuilderJob(component);
			if (!item2.ShouldReleaseNow)
			{
				return (item, item2);
			}
		}
		return (null, Decision.ReleaseNow());
	}

	private static bool IsReachable(DemolishJob job, Accessible start, out float distance)
	{
		return job.GetComponent<ReachableDemolishable>().IsReachable(start, out distance);
	}
}
