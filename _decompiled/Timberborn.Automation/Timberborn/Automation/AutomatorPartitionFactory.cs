namespace Timberborn.Automation;

internal class AutomatorPartitionFactory
{
	private readonly AutomationPlanVersioner _automationPlanVersioner;

	private readonly AutomationDebugger _automationDebugger;

	public AutomatorPartitionFactory(AutomationPlanVersioner automationPlanVersioner, AutomationDebugger automationDebugger)
	{
		_automationPlanVersioner = automationPlanVersioner;
		_automationDebugger = automationDebugger;
	}

	public AutomatorPartition Create()
	{
		return new AutomatorPartition(new AutomationPlan(_automationPlanVersioner), _automationDebugger);
	}
}
