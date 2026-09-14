namespace Timberborn.TemplateSystem;

public class TemplateInstantiationOrderService
{
	private int _order;

	public int GetOrder()
	{
		return ++_order;
	}
}
