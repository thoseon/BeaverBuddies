using Timberborn.Debugging;

namespace Timberborn.DebuggingUI;

internal class DebuggingPanelResetter : IDevModule
{
	private readonly DebuggingPanel _debuggingPanel;

	private readonly ObjectDebuggingPanel _objectDebuggingPanel;

	public DebuggingPanelResetter(DebuggingPanel debuggingPanel, ObjectDebuggingPanel objectDebuggingPanel)
	{
		_debuggingPanel = debuggingPanel;
		_objectDebuggingPanel = objectDebuggingPanel;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Reset debugging panels position", ResetPanelsPosition)).Build();
	}

	private void ResetPanelsPosition()
	{
		_debuggingPanel.ResetPanelPosition();
		_objectDebuggingPanel.ResetPanelPosition();
	}
}
