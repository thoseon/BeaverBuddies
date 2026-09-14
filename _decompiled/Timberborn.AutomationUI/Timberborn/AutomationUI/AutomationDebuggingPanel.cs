using System.Text;
using Timberborn.Automation;
using Timberborn.Common;
using Timberborn.DebuggingUI;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.AutomationUI;

internal class AutomationDebuggingPanel : ILoadableSingleton, IDebuggingPanel
{
	private readonly AutomationDebugger _automationDebugger;

	private readonly IAutomationRunnerDebugger _automationRunnerDebugger;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly DebuggingPanel _debuggingPanel;

	private readonly StringBuilder _stringBuilder = new StringBuilder();

	public AutomationDebuggingPanel(AutomationDebugger automationDebugger, IAutomationRunnerDebugger automationRunnerDebugger, EntitySelectionService entitySelectionService, DebuggingPanel debuggingPanel)
	{
		_automationDebugger = automationDebugger;
		_automationRunnerDebugger = automationRunnerDebugger;
		_entitySelectionService = entitySelectionService;
		_debuggingPanel = debuggingPanel;
	}

	public void Load()
	{
		_debuggingPanel.AddDebuggingPanel(this, "Automation");
	}

	public string GetText()
	{
		_stringBuilder.Clear();
		_stringBuilder.AppendLine(FormatMetric("Partitioning", _automationDebugger.PartitioningTimeMs));
		_stringBuilder.AppendLine(FormatMetric("Adding", _automationDebugger.AddingTimeMs));
		_stringBuilder.AppendLine(FormatMetric("Removing", _automationDebugger.RemovingTimeMs));
		_stringBuilder.AppendLine(FormatMetric("Merging", _automationDebugger.MergingTimeMs));
		_stringBuilder.AppendLine(FormatMetric("Planning", _automationDebugger.PlanningTimeMs));
		_stringBuilder.AppendLine(FormatMetric("Evaluation", _automationDebugger.EvaluationTimeMs));
		_stringBuilder.AppendLine(FormatMetric("Tick evaluation", _automationDebugger.TickEvaluationTimeMs));
		_stringBuilder.AppendLine($"Total partitions: {_automationRunnerDebugger.PartitionCount}");
		if (_entitySelectionService.IsAnythingSelected && _entitySelectionService.SelectedObject.TryGetComponent<Automator>(out var component))
		{
			AutomatorPartition partition = component.Partition;
			_stringBuilder.AppendLine();
			_stringBuilder.AppendLine("<b>Selected</b>");
			_stringBuilder.AppendLine((partition != null) ? $"Partition: {partition.DebuggingId} ({partition.Size} nodes)" : "No partition!");
			if (component.IsCyclicOrBlocked)
			{
				_stringBuilder.AppendLine("IsCyclicOrBlocked");
			}
			_stringBuilder.AppendLine($"Evaluations: {component.Evaluations}");
		}
		return _stringBuilder.ToStringWithoutNewLineEnd();
	}

	private static string FormatMetric(string name, AutomationDebuggerMetric metric)
	{
		return $"{name}: {metric.Total:0.0000}ms (max {metric.Max:0.0000}ms)";
	}
}
