using Timberborn.TickSystem;

namespace Timberborn.Automation;

public class AutomationDebugger : ITickableSingleton
{
	public AutomationDebuggerMetric PartitioningTimeMs { get; } = new AutomationDebuggerMetric();

	public AutomationDebuggerMetric AddingTimeMs { get; } = new AutomationDebuggerMetric();

	public AutomationDebuggerMetric RemovingTimeMs { get; } = new AutomationDebuggerMetric();

	public AutomationDebuggerMetric MergingTimeMs { get; } = new AutomationDebuggerMetric();

	public AutomationDebuggerMetric PlanningTimeMs { get; } = new AutomationDebuggerMetric();

	public AutomationDebuggerMetric EvaluationTimeMs { get; } = new AutomationDebuggerMetric();

	public AutomationDebuggerMetric TickEvaluationTimeMs { get; } = new AutomationDebuggerMetric();

	public void Tick()
	{
		PartitioningTimeMs.Reset();
		AddingTimeMs.Reset();
		RemovingTimeMs.Reset();
		MergingTimeMs.Reset();
		PlanningTimeMs.Reset();
		EvaluationTimeMs.Reset();
		TickEvaluationTimeMs.Reset();
	}
}
