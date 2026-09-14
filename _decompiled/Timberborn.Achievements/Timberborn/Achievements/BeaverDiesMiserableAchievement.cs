using Timberborn.AchievementSystem;
using Timberborn.Beavers;
using Timberborn.Characters;
using Timberborn.NeedSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class BeaverDiesMiserableAchievement : Achievement
{
	private static readonly string HungerNeedId = "Hunger";

	private static readonly string ThirstNeedId = "Thirst";

	private static readonly string InjuryNeedId = "Injury";

	private static readonly string ContaminationNeedId = "BadwaterContamination";

	private readonly EventBus _eventBus;

	public override string Id => "BEAVER_DIES_HUNGRY_THIRSTY_INJURED_SICK";

	public BeaverDiesMiserableAchievement(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	[OnEvent]
	public void OnCharacterKilled(CharacterKilledEvent characterKilledEvent)
	{
		CheckUnlockConditions(characterKilledEvent.Character);
	}

	protected override void EnableInternal()
	{
		_eventBus.Register(this);
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
	}

	private void CheckUnlockConditions(Character character)
	{
		if (character.HasComponent<BeaverSpec>())
		{
			NeedManager component = character.GetComponent<NeedManager>();
			if (component.NeedIsActive(HungerNeedId) && component.NeedIsActive(ThirstNeedId) && component.NeedIsActive(InjuryNeedId) && component.NeedIsActive(ContaminationNeedId))
			{
				Unlock();
			}
		}
	}
}
