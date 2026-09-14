using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.Effects;
using Timberborn.EnterableSystem;
using Timberborn.GameDistricts;
using Timberborn.NeedBehaviorSystem;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;
using Timberborn.TimeSystem;
using Timberborn.WalkingSystem;
using UnityEngine;

namespace Timberborn.Attractions;

public class AttractionNeedBehavior : NeedBehavior, IAwakableComponent, IFinishedStateListener
{
	private static readonly float MaxValueTolerance = 0.001f;

	private static readonly float MinHoursSpentInside = 0.5f;

	private readonly IDayNightCycle _dayNightCycle;

	private Enterable _enterable;

	private Attraction _attraction;

	private BuildingAccessible _buildingAccessible;

	private DistrictBuilding _districtBuilding;

	private DistrictNeedBehaviorService _districtNeedBehaviorService;

	private readonly List<ContinuousEffect> _effects = new List<ContinuousEffect>();

	public AttractionNeedBehavior(IDayNightCycle dayNightCycle)
	{
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		_enterable = GetComponent<Enterable>();
		_attraction = GetComponent<Attraction>();
		_buildingAccessible = GetComponent<BuildingAccessible>();
		_districtBuilding = GetComponent<DistrictBuilding>();
		DisableComponent();
	}

	public override Vector3? ActionPosition(NeedManager needManager)
	{
		Enterer component = needManager.GetComponent<Enterer>();
		if (_attraction.IsUsable && (_enterable.CanReserveSlot || component.CurrentBuilding == _enterable))
		{
			Vector3? unblockedSingleAccess = _buildingAccessible.Accessible.UnblockedSingleAccess;
			if (unblockedSingleAccess.HasValue)
			{
				return unblockedSingleAccess.GetValueOrDefault();
			}
		}
		return null;
	}

	public void OnEnterFinishedState()
	{
		_districtBuilding.ReassignedDistrict += OnReassignedDistrict;
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		_districtBuilding.ReassignedDistrict -= OnReassignedDistrict;
		RemoveDistrictNeedBehaviorService();
		DisableComponent();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (!_attraction.IsUsable || !base.Enabled || MaxValueNeedsAreActive(agent))
		{
			return Decision.ReleaseNextTick();
		}
		WalkInsideExecutor component = agent.GetComponent<WalkInsideExecutor>();
		AttractionAttender component2 = agent.GetComponent<AttractionAttender>();
		switch (component.Launch(_enterable))
		{
		case ExecutorStatus.Success:
		{
			ApplyEffectExecutor component3 = agent.GetComponent<ApplyEffectExecutor>();
			if (component2.FirstVisit)
			{
				component2.FirstVisit = false;
				return ApplyEffect(component3, MinHoursSpentInside);
			}
			return ApplyEffect(component3, 0.05f);
		}
		case ExecutorStatus.Failure:
			component2.FirstVisit = true;
			return Decision.ReleaseNextTick();
		case ExecutorStatus.Running:
			component2.FirstVisit = true;
			return Decision.ReturnWhenFinished(component);
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private Decision ApplyEffect(ApplyEffectExecutor applyEffectExecutor, float lengthOfStayInHours)
	{
		float timestamp = _dayNightCycle.DayNumberHoursFromNow(lengthOfStayInHours);
		_attraction.GetEfficiencyAdjustedEffects(_effects);
		applyEffectExecutor.LaunchToTimestamp(_effects, timestamp);
		_effects.Clear();
		if (!_attraction.SatisfiesAnyNeedToMaxValue)
		{
			return Decision.ReleaseWhenFinished(applyEffectExecutor);
		}
		return Decision.ReturnWhenFinished(applyEffectExecutor);
	}

	private void OnReassignedDistrict(object sender, EventArgs e)
	{
		RemoveDistrictNeedBehaviorService();
		DistrictCenter district = _districtBuilding.District;
		if ((bool)district)
		{
			_districtNeedBehaviorService = district.GetComponent<DistrictNeedBehaviorService>();
			_districtNeedBehaviorService.AddNeedBehavior(_attraction.Effects, this);
		}
	}

	private void RemoveDistrictNeedBehaviorService()
	{
		if ((bool)_districtNeedBehaviorService)
		{
			_districtNeedBehaviorService.RemoveNeedBehavior(_attraction.Effects, this);
			_districtNeedBehaviorService = null;
		}
	}

	private bool MaxValueNeedsAreActive(BehaviorAgent agent)
	{
		NeedManager component = agent.GetComponent<NeedManager>();
		bool result = false;
		IReadOnlyList<ContinuousEffectSpec> effects = _attraction.Effects;
		for (int i = 0; i < effects.Count; i++)
		{
			ContinuousEffectSpec continuousEffectSpec = effects[i];
			if (continuousEffectSpec.SatisfyToMaxValue)
			{
				if (component.NeedPointsToMax(continuousEffectSpec.NeedId) > MaxValueTolerance)
				{
					return false;
				}
				result = true;
			}
		}
		return result;
	}
}
