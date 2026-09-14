namespace Timberborn.Automation;

public class AutoAutomatableNeeder : IAutomatableNeeder
{
	private readonly AutomatorRegistry _automatorRegistry;

	public bool NeedsAutomatable => _automatorRegistry.AnyTransmitters();

	public AutoAutomatableNeeder(AutomatorRegistry automatorRegistry)
	{
		_automatorRegistry = automatorRegistry;
	}
}
