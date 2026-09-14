using Timberborn.CursorToolSystem;
using Timberborn.DebuggingUI;
using Timberborn.Navigation;
using Timberborn.SingletonSystem;

namespace Timberborn.NavigationUI;

public class NavigationDebuggingPanel : ILoadableSingleton, IDebuggingPanel
{
	private readonly DebuggingPanel _debuggingPanel;

	private readonly INavigationDebuggingService _navigationDebuggingService;

	private readonly CursorDebugger _cursorDebugger;

	public NavigationDebuggingPanel(DebuggingPanel debuggingPanel, INavigationDebuggingService navigationDebuggingService, CursorDebugger cursorDebugger)
	{
		_debuggingPanel = debuggingPanel;
		_navigationDebuggingService = navigationDebuggingService;
		_cursorDebugger = cursorDebugger;
	}

	public void Load()
	{
		_debuggingPanel.AddDebuggingPanel(this, "Navigation");
	}

	public string GetText()
	{
		return _navigationDebuggingService.InfoAt(_cursorDebugger.Position);
	}
}
