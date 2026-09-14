using Timberborn.GameSaveRepositorySystem;

namespace Timberborn.GameSaveRepositorySystemUI;

public class GameSaveItem
{
	public SaveReference SaveReference { get; }

	public string DisplayName { get; }

	public string Timestamp { get; }

	public string GameTime { get; }

	public bool IsAutosave { get; }

	public GameSaveItem(SaveReference saveReference, string displayName, string timestamp, string gameTime, bool isAutosave)
	{
		SaveReference = saveReference;
		DisplayName = displayName;
		Timestamp = timestamp;
		GameTime = gameTime;
		IsAutosave = isAutosave;
	}
}
