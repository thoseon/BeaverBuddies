using Timberborn.AchievementSystem;
using Timberborn.GameOver;
using Timberborn.HazardousWeatherSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class SurviveBadtideAchievement : Achievement
{
	private readonly EventBus _eventBus;

	private readonly IGameOverChecker _gameOverChecker;

	public override string Id => "SURVIVE_BADTIDE";

	public SurviveBadtideAchievement(EventBus eventBus, IGameOverChecker gameOverChecker)
	{
		_eventBus = eventBus;
		_gameOverChecker = gameOverChecker;
	}

	[OnEvent]
	public void OnHazardousWeatherEnded(HazardousWeatherEndedEvent hazardousWeatherEndedEvent)
	{
		if (!_gameOverChecker.IsGameOver() && hazardousWeatherEndedEvent.HazardousWeather is BadtideWeather)
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
