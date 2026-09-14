namespace Timberborn.Automation;

internal class AutomationPlanVersioner
{
	private long _planVersion;

	public long AcquirePlanVersion()
	{
		return ++_planVersion;
	}
}
