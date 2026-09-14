using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BlueprintSystem;
using Timberborn.NeedSystem;

namespace Timberborn.NeedSuspending;

internal record NeedSuspender
{
	[Serialize]
	public ImmutableArray<string> SuspendableNeedIds { get; init; }

	public void SuspendNeeds(BaseComponent component)
	{
		UpdateNeedSuspensions(component, shouldSuspend: true);
	}

	public void ResumeNeeds(BaseComponent component)
	{
		UpdateNeedSuspensions(component, shouldSuspend: false);
	}

	private void UpdateNeedSuspensions(BaseComponent component, bool shouldSuspend)
	{
		NeedManager component2 = component.GetComponent<NeedManager>();
		if (component2 == null)
		{
			return;
		}
		foreach (string suspendableNeedId in SuspendableNeedIds)
		{
			if (shouldSuspend)
			{
				component2.DisableUpdate(suspendableNeedId);
			}
			else
			{
				component2.EnableUpdate(suspendableNeedId);
			}
		}
	}
}
