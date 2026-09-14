using Timberborn.AchievementSystem;
using Timberborn.Beavers;
using Timberborn.BotUpkeep;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class BuildBotAfterBeaverExtinctionAchievement : Achievement
{
	private readonly EventBus _eventBus;

	private readonly BeaverPopulation _beaverPopulation;

	public override string Id => "BUILD_BOT_AFTER_BEAVER_EXTINCTION";

	public BuildBotAfterBeaverExtinctionAchievement(EventBus eventBus, BeaverPopulation beaverPopulation)
	{
		_eventBus = eventBus;
		_beaverPopulation = beaverPopulation;
	}

	[OnEvent]
	public void OnBotManufactured(BotManufacturedEvent botManufacturedEvent)
	{
		if (_beaverPopulation.NumberOfBeavers == 0)
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
