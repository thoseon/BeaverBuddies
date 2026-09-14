namespace Timberborn.GameStartup;

public class SettlementNameChangedEvent
{
	public string SettlementName { get; }

	public SettlementNameChangedEvent(string settlementName)
	{
		SettlementName = settlementName;
	}
}
