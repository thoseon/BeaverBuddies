namespace Timberborn.TutorialSystemUI;

internal class TutorialHeaderClickedEvent
{
	public string TutorialId { get; }

	public TutorialHeaderClickedEvent(string tutorialId)
	{
		TutorialId = tutorialId;
	}
}
