using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.StatusSystem;
using Timberborn.TooltipSystem;

namespace Timberborn.StatusSystemUI;

public class StatusBatchControlRowItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	public StatusBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		StatusSubject component = entity.GetComponent<StatusSubject>();
		if (component != null)
		{
			string elementName = "Game/BatchControl/StatusBatchControlRowItem";
			return new StatusBatchControlRowItem(_visualElementLoader.LoadVisualElement(elementName), component, _visualElementLoader, _tooltipRegistrar);
		}
		return null;
	}
}
