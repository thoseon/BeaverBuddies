namespace Timberborn.TutorialSystem;

public class TutorialCreatedEvent
{
	public TutorialConfiguration Configuration { get; }

	public TutorialCreatedEvent(TutorialConfiguration configuration)
	{
		Configuration = configuration;
	}
}
