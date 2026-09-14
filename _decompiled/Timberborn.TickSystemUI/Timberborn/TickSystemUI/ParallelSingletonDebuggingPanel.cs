using Timberborn.Debugging;
using Timberborn.DebuggingUI;
using Timberborn.Multithreading;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;

namespace Timberborn.TickSystemUI;

internal class ParallelSingletonDebuggingPanel : IDebuggingPanel, ILoadableSingleton, ITickableSingleton
{
	private readonly DebuggingPanel _debuggingPanel;

	private readonly ITickableSingletonService _tickableSingletonService;

	private readonly IParallelizer _parallelizer;

	private readonly DebugModeManager _debugModeManager;

	private string _text;

	public ParallelSingletonDebuggingPanel(DebuggingPanel debuggingPanel, ITickableSingletonService tickableSingletonService, DebugModeManager debugModeManager, IParallelizer parallelizer)
	{
		_debuggingPanel = debuggingPanel;
		_tickableSingletonService = tickableSingletonService;
		_debugModeManager = debugModeManager;
		_parallelizer = parallelizer;
	}

	public void Load()
	{
		_debuggingPanel.AddDebuggingPanel(this, "Parallel singletons");
		_text = GetText("Waiting for tick...");
	}

	public void Tick()
	{
		if (_debugModeManager.Enabled)
		{
			_text = GetText($"Total time: {_tickableSingletonService.LastParallelTickDuration.TotalMilliseconds:F0}ms");
		}
	}

	public string GetText()
	{
		return _text;
	}

	private string GetText(string timeText)
	{
		return $"Number of threads: {_parallelizer.NumberOfThreads}\n{timeText}";
	}
}
