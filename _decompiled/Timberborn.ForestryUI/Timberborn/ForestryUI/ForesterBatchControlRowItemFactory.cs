using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.Forestry;

namespace Timberborn.ForestryUI;

public class ForesterBatchControlRowItemFactory
{
	private static readonly string ButtonClass = "forester-batch-control-row-item";

	private static readonly string ReplantDeadTreesLocKey = "Planting.ReplantDeadTrees";

	private readonly ToggleButtonBatchControlRowItemFactory _toggleButtonFactory;

	public ForesterBatchControlRowItemFactory(ToggleButtonBatchControlRowItemFactory toggleButtonFactory)
	{
		_toggleButtonFactory = toggleButtonFactory;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		Forester component = entity.GetComponent<Forester>();
		if (!component)
		{
			return null;
		}
		return Create(component);
	}

	private IBatchControlRowItem Create(Forester forester)
	{
		return _toggleButtonFactory.Create(ButtonClass, delegate
		{
			forester.SetReplantDeadTrees(!forester.ReplantDeadTrees);
		}, () => forester.ReplantDeadTrees, ReplantDeadTreesLocKey);
	}
}
