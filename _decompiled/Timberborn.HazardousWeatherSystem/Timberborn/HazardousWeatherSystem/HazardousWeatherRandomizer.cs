using Timberborn.Common;
using UnityEngine;

namespace Timberborn.HazardousWeatherSystem;

public class HazardousWeatherRandomizer
{
	private static readonly float StreakChanceDecreaseThreshold = 0.05f;

	private static readonly float StreakChanceResetThreshold = 0.025f;

	private static readonly float DecreaseRatio = 0.5f;

	private readonly DroughtWeather _droughtWeather;

	private readonly BadtideWeather _badtideWeather;

	private readonly HazardousWeatherHistory _hazardousWeatherHistory;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private float BaseBadtideChance => _badtideWeather.ChanceForBadtide;

	private bool IsBadtideStreak => _hazardousWeatherHistory.CurrentStreakId == _badtideWeather.Id;

	public HazardousWeatherRandomizer(DroughtWeather droughtWeather, BadtideWeather badtideWeather, HazardousWeatherHistory hazardousWeatherHistory, IRandomNumberGenerator randomNumberGenerator)
	{
		_droughtWeather = droughtWeather;
		_badtideWeather = badtideWeather;
		_hazardousWeatherHistory = hazardousWeatherHistory;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public IHazardousWeather GetRandomWeatherForCycle(int cycle)
	{
		if (ShouldBeBadtideWeather(cycle))
		{
			return _badtideWeather;
		}
		return _droughtWeather;
	}

	private bool ShouldBeBadtideWeather(int cycle)
	{
		if (_badtideWeather.CanOccurAtCycle(cycle))
		{
			return _randomNumberGenerator.CheckProbability(GetBadtideChance());
		}
		return false;
	}

	private float GetBadtideChance()
	{
		if (_hazardousWeatherHistory.CurrentStreak <= 0)
		{
			return BaseBadtideChance;
		}
		return GetModifiedBadtideChance();
	}

	private float GetModifiedBadtideChance()
	{
		float num = (IsBadtideStreak ? BaseBadtideChance : (1f - BaseBadtideChance));
		float num2 = Mathf.Pow(num, _hazardousWeatherHistory.CurrentStreak + 1);
		if (num2 < StreakChanceResetThreshold)
		{
			num = 0f;
		}
		else if (num2 < StreakChanceDecreaseThreshold)
		{
			num *= DecreaseRatio;
		}
		if (!IsBadtideStreak)
		{
			return 1f - num;
		}
		return num;
	}
}
