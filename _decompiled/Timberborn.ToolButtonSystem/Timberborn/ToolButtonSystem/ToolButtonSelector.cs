using Timberborn.InputSystem;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using Timberborn.UILayoutSystem;

namespace Timberborn.ToolButtonSystem;

internal class ToolButtonSelector : IInputProcessor, ILoadableSingleton
{
	private static readonly string NextRootButtonKey = "NextRootButton";

	private static readonly string PreviousRootButtonKey = "PreviousRootButton";

	private static readonly string NextToolKey = "NextTool";

	private static readonly string PreviousToolKey = "PreviousTool";

	private static readonly string UnlockToolKey = "UnlockTool";

	private readonly InputService _inputService;

	private readonly ToolButtonService _toolButtonService;

	private readonly ToolService _toolService;

	private readonly ToolUnlockingService _toolUnlockingService;

	private readonly UILayout _uiLayout;

	private readonly EventBus _eventBus;

	public ToolButtonSelector(InputService inputService, ToolButtonService toolButtonService, ToolService toolService, ToolUnlockingService toolUnlockingService, UILayout uiLayout, EventBus eventBus)
	{
		_inputService = inputService;
		_toolButtonService = toolButtonService;
		_toolService = toolService;
		_toolUnlockingService = toolUnlockingService;
		_uiLayout = uiLayout;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public bool ProcessInput()
	{
		if (_uiLayout.BottomBarVisible)
		{
			return ProcessToolbarInput();
		}
		return false;
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_inputService.AddInputProcessor(this);
	}

	[OnEvent]
	public void OnToolGroupEntered(ToolGroupEnteredEvent toolGroupEntered)
	{
		_toolService.SwitchToDefaultTool();
	}

	private bool ProcessToolbarInput()
	{
		if (_inputService.IsKeyDown(NextRootButtonKey) && _toolButtonService.TryGetNextRootButton(out var nextButton))
		{
			nextButton.Select();
			return true;
		}
		if (_inputService.IsKeyDown(PreviousRootButtonKey) && _toolButtonService.TryGetPreviousRootButton(out var previousButton))
		{
			previousButton.Select();
			return true;
		}
		if (_inputService.IsKeyDown(NextToolKey) && _toolButtonService.TryGetNextToolButton(out var toolButton))
		{
			toolButton.Select();
			return true;
		}
		if (_inputService.IsKeyDown(PreviousToolKey) && _toolButtonService.TryGetPreviousToolButton(out var toolButton2))
		{
			toolButton2.Select();
			return true;
		}
		if (_inputService.IsKeyDown(UnlockToolKey) && _toolUnlockingService.IsLocked(_toolService.ActiveTool))
		{
			_toolUnlockingService.TryToUnlock(_toolService.ActiveTool);
			return true;
		}
		return false;
	}
}
