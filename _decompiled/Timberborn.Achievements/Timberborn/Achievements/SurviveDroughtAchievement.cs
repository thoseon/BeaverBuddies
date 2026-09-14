using Timberborn.AchievementSystem;
using Timberborn.GameOver;
using Timberborn.HazardousWeatherSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class SurviveDroughtAchievement : Achievement
{
	private readonly EventBus _eventBus;

	private readonly IGameOverChecker _gameOverChecker;

	public override string Id => "SURVIVE_DROUGHT";

	public SurviveDroughtAchievement(EventBus eventBus, IGameOverChecker gameOverChecker)
	{
		_eventBus = eventBus;
		_gameOverChecker = gameOverChecker;
	}

	[OnEvent]
	public void OnHazardousWeatherEnded(HazardousWeatherEndedEvent hazardousWeatherEndedEvent)
	{
		if (!_gameOverChecker.IsGameOver() && hazardousWeatherEndedEvent.HazardousWeather is DroughtWeather)
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
