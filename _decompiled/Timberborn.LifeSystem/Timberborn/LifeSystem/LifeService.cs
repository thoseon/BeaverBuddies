using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;
using UnityEngine;

namespace Timberborn.LifeSystem;

public class LifeService : ILoadableSingleton
{
	private readonly IDayNightCycle _dayNightCycle;

	private readonly ISpecService _specService;

	private int _averageLifespan;

	private int _daysOfChildhood;

	public float LifeProgressIncreasePerTick { get; private set; }

	private int DaysOfAdulthood => _averageLifespan - _daysOfChildhood;

	public LifeService(IDayNightCycle dayNightCycle, ISpecService specService)
	{
		_dayNightCycle = dayNightCycle;
		_specService = specService;
	}

	public void Load()
	{
		LifeServiceSpec singleSpec = _specService.GetSingleSpec<LifeServiceSpec>();
		_averageLifespan = singleSpec.AverageLifespan;
		_daysOfChildhood = singleSpec.DaysOfChildhood;
		LifeProgressIncreasePerTick = 1f / 24f * _dayNightCycle.FixedDeltaTimeInHours / (float)_averageLifespan;
	}

	public float ChildhoodProgressToLifeProgress(float childhoodProgress)
	{
		return childhoodProgress * (float)_daysOfChildhood / (float)_averageLifespan;
	}

	public float AdulthoodProgressToLifeProgress(float adulthoodProgress)
	{
		return (adulthoodProgress * (float)DaysOfAdulthood + (float)_daysOfChildhood) / (float)_averageLifespan;
	}

	public int CalculateDayOfBirth(float lifeProgress)
	{
		return _dayNightCycle.DayNumber - Mathf.RoundToInt(lifeProgress * (float)_averageLifespan);
	}

	public float CalculateGrowthProgress(float deltaTimeInHours)
	{
		int num = _daysOfChildhood * 24;
		return deltaTimeInHours / (float)num;
	}
}
