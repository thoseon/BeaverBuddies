namespace Timberborn.GameDistrictsMigration;

public class ManualMigrationBlockingStateChangedEvent
{
	public bool IsEnabled { get; }

	public ManualMigrationBlockingStateChangedEvent(bool isEnabled)
	{
		IsEnabled = isEnabled;
	}
}
