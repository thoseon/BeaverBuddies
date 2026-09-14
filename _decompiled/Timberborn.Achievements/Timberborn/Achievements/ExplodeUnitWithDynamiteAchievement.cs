using Timberborn.AchievementSystem;
using Timberborn.Explosions;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class ExplodeUnitWithDynamiteAchievement : Achievement
{
	private readonly EventBus _eventBus;

	public override string Id => "EXPLODE_UNIT_WITH_DYNAMITE";

	public ExplodeUnitWithDynamiteAchievement(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	[OnEvent]
	public void OnMortalDiedFromExplosionEvent(MortalDiedFromExplosionEvent mortalDiedFromExplosionEvent)
	{
		if (mortalDiedFromExplosionEvent.Source?.GetComponent<Dynamite>() != null)
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
}
