using Timberborn.AchievementSystem;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.Wonders;

namespace Timberborn.Achievements;

internal class ActivateMultipleWondersAchievement : Achievement
{
	private static readonly int RequiredActiveWonders = 3;

	private readonly EventBus _eventBus;

	private readonly EntityComponentRegistry _entityComponentRegistry;

	public override string Id => "ACTIVATE_MULTIPLE_WONDERS";

	public ActivateMultipleWondersAchievement(EventBus eventBus, EntityComponentRegistry entityComponentRegistry)
	{
		_eventBus = eventBus;
		_entityComponentRegistry = entityComponentRegistry;
	}

	[OnEvent]
	public void OnWonderActivated(WonderActivatedEvent wonderActivatedEvent)
	{
		if (GetActiveWonderCount() >= RequiredActiveWonders)
		{
			Unlock();
		}
	}

	protected override void EnableInternal()
	{
		_eventBus.Register(this);
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
	}

	private int GetActiveWonderCount()
	{
		int num = 0;
		foreach (Wonder item in _entityComponentRegistry.GetEnabled<Wonder>())
		{
			if (item.IsActive)
			{
				num++;
			}
		}
		return num;
	}
}
