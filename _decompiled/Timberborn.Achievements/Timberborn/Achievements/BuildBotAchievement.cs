using Timberborn.AchievementSystem;
using Timberborn.BotUpkeep;
using Timberborn.Bots;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class BuildBotAchievement : Achievement
{
	private readonly EventBus _eventBus;

	private readonly BotPopulation _botPopulation;

	public override string Id => "BUILD_BOT";

	public BuildBotAchievement(EventBus eventBus, BotPopulation botPopulation)
	{
		_eventBus = eventBus;
		_botPopulation = botPopulation;
	}

	[OnEvent]
	public void OnBotManufactured(BotManufacturedEvent botManufacturedEvent)
	{
		Unlock();
	}

	protected override void EnableInternal()
	{
		if (_botPopulation.BotCreated)
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
