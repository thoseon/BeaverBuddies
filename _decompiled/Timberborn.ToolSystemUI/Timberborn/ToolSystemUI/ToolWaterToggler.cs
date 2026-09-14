using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using Timberborn.WaterSystemRendering;

namespace Timberborn.ToolSystemUI;

internal class ToolWaterToggler : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly ToolService _toolService;

	private readonly WaterOpacityService _waterOpacityService;

	private WaterOpacityToggle _waterOpacityToggle;

	public ToolWaterToggler(EventBus eventBus, ToolService toolService, WaterOpacityService waterOpacityService)
	{
		_eventBus = eventBus;
		_toolService = toolService;
		_waterOpacityService = waterOpacityService;
	}

	public void Load()
	{
		_waterOpacityToggle = _waterOpacityService.GetWaterOpacityToggle();
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnToolGroupEntered(ToolGroupEnteredEvent toolGroupEnteredEvent)
	{
		if (toolGroupEnteredEvent.ToolGroup != null)
		{
			_waterOpacityToggle.HideWater();
		}
	}

	[OnEvent]
	public void OnToolGroupExited(ToolGroupExitedEvent toolGroupExitedEvent)
	{
		_waterOpacityToggle.ShowWater();
	}

	[OnEvent]
	public void OnToolEntered(ToolEnteredEvent toolEnteredEvent)
	{
		if (!_toolService.IsDefaultToolActive && !(toolEnteredEvent.Tool is IWaterIgnoringTool))
		{
			_waterOpacityToggle.HideWater();
		}
	}

	[OnEvent]
	public void OnToolExited(ToolExitedEvent toolExitedEvent)
	{
		_waterOpacityToggle.ShowWater();
	}
}
