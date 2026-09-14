using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.Hauling;

namespace Timberborn.HaulingUI;

public class HaulCandidateBatchControlRowItemFactory
{
	private static readonly string ButtonClass = "haul-candidate-batch-control-row-item";

	private static readonly string PrioritizeLocKey = "Hauling.Prioritize";

	private readonly ToggleButtonBatchControlRowItemFactory _toggleButtonBatchControlRowItemFactory;

	public HaulCandidateBatchControlRowItemFactory(ToggleButtonBatchControlRowItemFactory toggleButtonBatchControlRowItemFactory)
	{
		_toggleButtonBatchControlRowItemFactory = toggleButtonBatchControlRowItemFactory;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		HaulCandidateUIBlocker component = entity.GetComponent<HaulCandidateUIBlocker>();
		if ((bool)component && component.ShouldShowUI())
		{
			HaulPrioritizable haulPrioritizable = component.GetComponent<HaulPrioritizable>();
			return _toggleButtonBatchControlRowItemFactory.Create(ButtonClass, delegate
			{
				haulPrioritizable.Prioritized = !haulPrioritizable.Prioritized;
			}, () => haulPrioritizable.Prioritized, PrioritizeLocKey);
		}
		return null;
	}
}
