using System.Linq;
using Timberborn.AchievementSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;

namespace Timberborn.Achievements;

internal class BuildManyHedgesAchievement : Achievement
{
	private static readonly int HedgesRequired = 200;

	private static readonly string TemplateName = "Hedge.Folktails";

	private readonly EventBus _eventBus;

	private readonly EntityComponentRegistry _entityComponentRegistry;

	private int _hedgeCount;

	public override string Id => "BUILD_MANY_HEDGES";

	public BuildManyHedgesAchievement(EventBus eventBus, EntityComponentRegistry entityComponentRegistry)
	{
		_eventBus = eventBus;
		_entityComponentRegistry = entityComponentRegistry;
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		TemplateSpec component = enteredFinishedStateEvent.BlockObject.GetComponent<TemplateSpec>();
		if (component != null && component.TemplateName == TemplateName && ++_hedgeCount >= HedgesRequired)
		{
			Unlock();
		}
	}

	[OnEvent]
	public void OnExitedFinishedState(ExitedFinishedStateEvent exitedFinishedStateEvent)
	{
		TemplateSpec component = exitedFinishedStateEvent.BlockObject.GetComponent<TemplateSpec>();
		if (component != null && component.TemplateName == TemplateName)
		{
			_hedgeCount--;
		}
	}

	protected override void EnableInternal()
	{
		_eventBus.Register(this);
		ValidateInitialHedges();
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
	}

	private void ValidateInitialHedges()
	{
		_hedgeCount = (from spec in _entityComponentRegistry.GetEnabled<Building>()
			where spec.GetComponent<TemplateSpec>().TemplateName == TemplateName
			select spec).Count((Building spec) => spec.GetComponent<BlockObject>().IsFinished);
		if (_hedgeCount >= HedgesRequired)
		{
			Unlock();
		}
	}
}
