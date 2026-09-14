using Timberborn.MapRepositorySystem;

namespace Timberborn.NewGameConfigurationSystem;

public class NewGameConfiguration
{
	public string FactionId { get; }

	public MapFileReference MapFileReference { get; }

	public GameModeSpec GameMode { get; }

	public string SettlementName { get; }

	public NewGameConfiguration(string factionId, MapFileReference mapFileReference, GameModeSpec gameMode, string settlementName)
	{
		FactionId = factionId;
		MapFileReference = mapFileReference;
		GameMode = gameMode;
		SettlementName = settlementName;
	}

	public override string ToString()
	{
		return "FactionId: " + FactionId + ", " + string.Format("{0}: {1}, ", "MapFileReference", MapFileReference) + string.Format("{0}: {1}", "GameMode", GameMode);
	}
}
