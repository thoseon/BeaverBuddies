namespace Timberborn.EntityPanelSystem;

public class OrderedEntityPanelFragment
{
	public IEntityPanelFragment Fragment { get; }

	public int Order { get; }

	public OrderedEntityPanelFragment(IEntityPanelFragment fragment, int order)
	{
		Fragment = fragment;
		Order = order;
	}
}
