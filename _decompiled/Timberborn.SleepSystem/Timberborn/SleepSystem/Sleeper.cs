using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Beavers;
using Timberborn.BehaviorSystem;
using Timberborn.Common;
using Timberborn.Effects;
using Timberborn.NeedBehaviorSystem;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;
using Timberborn.TimeSystem;

namespace Timberborn.SleepSystem;

public class Sleeper : BaseComponent, IAwakableComponent
{
	public static readonly string SleepNeedId = "Sleep";

	private static readonly string SleepAnimationName = "Sleeping";

	private readonly IDayNightCycle _dayNightCycle;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private ApplyEffectExecutor _applyEffectExecutor;

	private NeedManager _needManager;

	private Child _child;

	private SleeperSpec _sleeperSpec;

	public ImmutableArray<ContinuousEffectSpec> SleepOutsideEffects => _sleeperSpec.SleepOutsideEffects;

	public bool IsNewborn
	{
		get
		{
			if ((bool)_child)
			{
				return _child.IsNewborn;
			}
			return false;
		}
	}

	public Sleeper(IDayNightCycle dayNightCycle, IRandomNumberGenerator randomNumberGenerator)
	{
		_dayNightCycle = dayNightCycle;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void Awake()
	{
		_needManager = GetComponent<NeedManager>();
		_child = GetComponent<Child>();
		_sleeperSpec = GetComponent<SleeperSpec>();
		_applyEffectExecutor = GetComponent<ApplyEffectExecutor>();
	}

	public bool ShouldSleepCritically()
	{
		return _needManager.NeedIsAtMinimumPoints(SleepNeedId);
	}

	public IExecutor LaunchExecutor(IEnumerable<ContinuousEffectSpec> sleepEffectsSpecs)
	{
		List<ContinuousEffect> list = ToSleepEffects(sleepEffectsSpecs).ToList();
		float timestamp = CalculateWakeUpTimestamp(list);
		_applyEffectExecutor.LaunchToTimestamp(list, timestamp, SleepAnimationName);
		return _applyEffectExecutor;
	}

	private IEnumerable<ContinuousEffect> ToSleepEffects(IEnumerable<ContinuousEffectSpec> effectSpecs)
	{
		float scale = (ShouldSleepCritically() ? 0.66f : 1f);
		return effectSpecs.Select((ContinuousEffectSpec effectSpec) => new ContinuousEffect(effectSpec.NeedId, effectSpec.PointsPerHour * scale));
	}

	private float CalculateWakeUpTimestamp(IReadOnlyList<ContinuousEffect> sleepEffects)
	{
		float num = _dayNightCycle.HoursToNextStartOf(TimeOfDay.Daytime);
		ContinuousEffect effect = sleepEffects.Single((ContinuousEffect continuousEffect) => continuousEffect.NeedId == SleepNeedId);
		float num2 = _needManager.FullyEffectiveDurationInHours(effect);
		float num3 = _randomNumberGenerator.Range(0f, _sleeperSpec.MaxOffsetInHours);
		if (!(Math.Abs(num - num2) < 1f) && !IsNewborn)
		{
			return _dayNightCycle.DayNumberHoursFromNow(num2 + num3);
		}
		return _dayNightCycle.DayNumberHoursFromNow(num + num3);
	}
}
