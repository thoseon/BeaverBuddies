using Timberborn.AchievementSystem;
using Timberborn.BeaverContaminationSystem;
using Timberborn.Characters;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class CureContaminatedBeaverAchievement : Achievement
{
	private readonly EventBus _eventBus;

	public override string Id => "CURE_CONTAMINATED_BEAVER";

	public CureContaminatedBeaverAchievement(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	[OnEvent]
	public void OnContaminableContaminationChanged(ContaminableContaminationChangedEvent contaminableContaminationChangedEvent)
	{
		Contaminable contaminable = contaminableContaminationChangedEvent.Contaminable;
		if (!contaminable.IsContaminated && contaminable.GetComponent<Character>().Alive)
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
