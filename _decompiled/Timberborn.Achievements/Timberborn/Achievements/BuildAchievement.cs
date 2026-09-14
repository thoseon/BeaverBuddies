using System.Linq;
using Timberborn.AchievementSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;

namespace Timberborn.Achievements;

internal abstract class BuildAchievement : Achievement
{
	private readonly EventBus _eventBus;

	private readonly EntityComponentRegistry _entityComponentRegistry;

	private readonly string _requiredPrefix;

	public override string Id { get; }

	protected BuildAchievement(EventBus eventBus, EntityComponentRegistry entityComponentRegistry, string id, string requiredPrefix)
	{
		Id = id;
		_eventBus = eventBus;
		_entityComponentRegistry = entityComponentRegistry;
		_requiredPrefix = requiredPrefix;
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		TemplateSpec component = enteredFinishedStateEvent.BlockObject.GetComponent<TemplateSpec>();
		if (component != null && component.TemplateName.StartsWith(_requiredPrefix))
		{
			Unlock();
		}
	}

	protected override void EnableInternal()
	{
		if ((from s in _entityComponentRegistry.GetEnabled<Building>()
			where s.GetComponent<BlockObject>().IsFinished
			select s).Any((Building s) => s.GetComponent<TemplateSpec>().TemplateName.StartsWith(_requiredPrefix)))
		{
			Unlock();
		}
		else
		{
			_eventBus.Register(this);
		}
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
	}
}
