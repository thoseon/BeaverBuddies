namespace Timberborn.TutorialSystem;

public class TutorialStageStartedEvent
{
	public string TutorialId { get; }

	public TutorialStage TutorialStage { get; }

	public TutorialStageStartedEvent(string tutorialId, TutorialStage tutorialStage)
	{
		TutorialId = tutorialId;
		TutorialStage = tutorialStage;
	}
}
