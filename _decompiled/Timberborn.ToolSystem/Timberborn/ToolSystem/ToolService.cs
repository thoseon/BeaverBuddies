using Timberborn.InputSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.ToolSystem;

public class ToolService : IInputProcessor, ILoadableSingleton, IPostLoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly InputService _inputService;

	private readonly ToolGroupService _toolGroupService;

	private readonly IDefaultToolProvider _defaultToolProvider;

	private ITool _defaultTool;

	public ITool ActiveTool { get; private set; }

	public bool IsDefaultToolActive => IsDefaultTool(ActiveTool);

	public ToolService(EventBus eventBus, InputService inputService, ToolGroupService toolGroupService, IDefaultToolProvider defaultToolProvider)
	{
		_eventBus = eventBus;
		_inputService = inputService;
		_toolGroupService = toolGroupService;
		_defaultToolProvider = defaultToolProvider;
	}

	public void Load()
	{
		_eventBus.Register(this);
		_defaultTool = _defaultToolProvider.DefaultTool;
	}

	public void PostLoad()
	{
		SwitchToDefaultTool();
	}

	public bool ProcessInput()
	{
		if (_inputService.Cancel && !IsDefaultToolActive)
		{
			SwitchToDefaultTool();
			return true;
		}
		return false;
	}

	public void SwitchToDefaultTool()
	{
		SwitchToolInternal(_defaultTool, shouldCloseGroup: false, forceSwitch: false);
	}

	public void SwitchTool(ITool tool)
	{
		SwitchToolInternal(tool, ShouldCloseGroup(tool), IsDefaultTool(tool));
	}

	public void ExitTool()
	{
		if (ActiveTool != null)
		{
			ITool activeTool = ActiveTool;
			ActiveTool.Exit();
			ActiveTool = null;
			_inputService.RemoveInputProcessor(this);
			_eventBus.Post(new ToolExitedEvent(activeTool));
		}
	}

	public void SetTemporaryTool(ITool tool)
	{
		_eventBus.Post(new TemporaryToolEnteredEvent(tool));
	}

	public void ClearTemporaryTool()
	{
		_eventBus.Post(new TemporaryToolExitedEvent());
	}

	private bool ShouldCloseGroup(ITool tool)
	{
		if (!(tool is IGroupIgnoringTool))
		{
			return !_toolGroupService.IsAssignedToAnyGroup(tool);
		}
		return false;
	}

	private bool IsDefaultTool(ITool tool)
	{
		return tool == _defaultTool;
	}

	private void SwitchToolInternal(ITool tool, bool shouldCloseGroup, bool forceSwitch)
	{
		if ((ActiveTool != tool) | forceSwitch)
		{
			ExitTool();
			_inputService.AddInputProcessor(this);
			ActiveTool = tool;
			tool.Enter();
			_eventBus.Post(new ToolEnteredEvent(tool, shouldCloseGroup));
		}
	}
}
