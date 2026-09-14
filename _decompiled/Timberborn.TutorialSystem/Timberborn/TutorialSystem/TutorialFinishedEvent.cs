namespace Timberborn.TutorialSystem;

public class TutorialFinishedEvent
{
	public string TutorialId { get; }

	public TutorialFinishedEvent(string tutorialId)
	{
		TutorialId = tutorialId;
	}
}
