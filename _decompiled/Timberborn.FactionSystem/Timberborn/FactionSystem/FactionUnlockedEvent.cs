namespace Timberborn.FactionSystem;

public class FactionUnlockedEvent
{
	public readonly FactionSpec Faction;

	public FactionUnlockedEvent(FactionSpec faction)
	{
		Faction = faction;
	}
}
