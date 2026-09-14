namespace Timberborn.MainMenuSceneLoading;

public class PreMainMenuStartedEvent
{
	public bool SkipAutoSave { get; }

	public PreMainMenuStartedEvent(bool skipAutoSave)
	{
		SkipAutoSave = skipAutoSave;
	}
}
