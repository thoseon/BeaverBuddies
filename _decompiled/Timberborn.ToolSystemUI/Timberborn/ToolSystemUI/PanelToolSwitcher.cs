using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;

namespace Timberborn.ToolSystemUI;

internal class PanelToolSwitcher : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly ToolService _toolService;

	public PanelToolSwitcher(EventBus eventBus, ToolService toolService)
	{
		_eventBus = eventBus;
		_toolService = toolService;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnPanelShown(PanelShownEvent panelShownEvent)
	{
		if (!panelShownEvent.IsDialog && !_toolService.IsDefaultToolActive)
		{
			_toolService.ExitTool();
		}
	}

	[OnEvent]
	public void OnPanelHidden(PanelHiddenEvent panelHiddenEvent)
	{
		if (!panelHiddenEvent.WasDialog && !panelHiddenEvent.AnyPanelShown)
		{
			_toolService.SwitchToDefaultTool();
		}
	}
}
