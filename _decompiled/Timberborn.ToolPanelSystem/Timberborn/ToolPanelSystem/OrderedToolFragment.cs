namespace Timberborn.ToolPanelSystem;

public readonly struct OrderedToolFragment
{
	public IToolFragment ToolFragment { get; }

	public int Order { get; }

	public OrderedToolFragment(IToolFragment toolFragment, int order)
	{
		ToolFragment = toolFragment;
		Order = order;
	}
}
