using Timberborn.DebuggingUI;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.TimeSystemUI;

public class TimeScaleDebuggingPanel : ILoadableSingleton, IDebuggingPanel
{
	private readonly DebuggingPanel _debuggingPanel;

	public TimeScaleDebuggingPanel(DebuggingPanel debuggingPanel)
	{
		_debuggingPanel = debuggingPanel;
	}

	public void Load()
	{
		_debuggingPanel.AddDebuggingPanel(this, "Time scale");
	}

	public string GetText()
	{
		return $"Real game speed (Time.timeScale): {Time.timeScale}";
	}
}
