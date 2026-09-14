using Timberborn.Buildings;
using Timberborn.ScienceSystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class StairsUnlockedTrigger : ILoadableSingleton
{
	private static readonly string TriggerId = "StairsUnlockedTrigger";

	private readonly EventBus _eventBus;

	private readonly ITutorialTriggers _tutorialTriggers;

	private readonly TemplateNameMapper _templateNameMapper;

	private BuildingSpec _buildingSpec;

	public StairsUnlockedTrigger(EventBus eventBus, ITutorialTriggers tutorialTriggers, TemplateNameMapper templateNameMapper)
	{
		_eventBus = eventBus;
		_tutorialTriggers = tutorialTriggers;
		_templateNameMapper = templateNameMapper;
	}

	public void Load()
	{
		if (_tutorialTriggers.TriggerPending(TriggerId))
		{
			TemplateSpec template = _templateNameMapper.GetTemplate("Stairs.Folktails");
			_buildingSpec = template.GetSpec<BuildingSpec>();
			_eventBus.Register(this);
		}
	}

	[OnEvent]
	public void OnBuildingUnlocked(BuildingUnlockedEvent buildingUnlockedEvent)
	{
		if (buildingUnlockedEvent.BuildingSpec == _buildingSpec)
		{
			_eventBus.Unregister(this);
			_tutorialTriggers.AddTrigger(TriggerId);
		}
	}
}
