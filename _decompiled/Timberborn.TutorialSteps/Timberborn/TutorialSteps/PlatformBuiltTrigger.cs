using System.Collections.Immutable;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class PlatformBuiltTrigger : ILoadableSingleton
{
	private static readonly string TriggerId = "PlatformBuiltTrigger";

	private static readonly ImmutableArray<string> PlatformTemplateNames = ImmutableArray.Create("Platform.Folktails", "DoublePlatform.Folktails", "TriplePlatform.Folktails");

	private readonly EventBus _eventBus;

	private readonly ITutorialTriggers _tutorialTriggers;

	public PlatformBuiltTrigger(EventBus eventBus, ITutorialTriggers tutorialTriggers)
	{
		_eventBus = eventBus;
		_tutorialTriggers = tutorialTriggers;
	}

	public void Load()
	{
		if (_tutorialTriggers.TriggerPending(TriggerId))
		{
			_eventBus.Register(this);
		}
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		Building component = enteredFinishedStateEvent.BlockObject.GetComponent<Building>();
		if (component != null)
		{
			string templateName = component.GetComponent<TemplateSpec>().TemplateName;
			if (PlatformTemplateNames.Contains(templateName))
			{
				_eventBus.Unregister(this);
				_tutorialTriggers.AddTrigger(TriggerId);
			}
		}
	}
}
