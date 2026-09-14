using Timberborn.BaseComponentSystem;

namespace Timberborn.TemplateSystem;

public class InstantiatedTemplate : BaseComponent, IAwakableComponent
{
	private readonly TemplateInstantiationOrderService _templateInstantiationOrderService;

	public int InstantiationOrder { get; private set; }

	public InstantiatedTemplate(TemplateInstantiationOrderService templateInstantiationOrderService)
	{
		_templateInstantiationOrderService = templateInstantiationOrderService;
	}

	public void Awake()
	{
		InstantiationOrder = _templateInstantiationOrderService.GetOrder();
	}
}
