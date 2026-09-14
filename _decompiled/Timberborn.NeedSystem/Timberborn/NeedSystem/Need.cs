using System;
using Timberborn.Effects;
using Timberborn.NeedSpecs;
using UnityEngine;

namespace Timberborn.NeedSystem;

public class Need
{
	private readonly float _deltaPointsPerUpdate;

	private readonly float _pointsWarningThreshold;

	private bool _appliedEffectSinceLastUpdate;

	private bool _updateEnabled = true;

	private readonly bool _isNeverPositive;

	public NeedSpec NeedSpec { get; }

	public bool IsCritical { get; }

	public float Points { get; private set; }

	public bool Enabled { get; private set; } = true;

	public bool IsInCriticalState
	{
		get
		{
			if (IsCritical)
			{
				return !IsFavorable;
			}
			return false;
		}
	}

	public bool IsFavorable
	{
		get
		{
			if (!_isNeverPositive)
			{
				return Points > 0f;
			}
			return Points == 0f;
		}
	}

	public bool IsBelowWarningThreshold => Points <= _pointsWarningThreshold;

	public bool IsActive
	{
		get
		{
			if (Enabled)
			{
				return Points != 0f;
			}
			return false;
		}
	}

	public bool IsAtMinimumPoints => Points <= NeedSpec.MinimumValue;

	public float PointsToMax => NeedSpec.MaximumValue - Points;

	public int Wellbeing
	{
		get
		{
			if (!Enabled)
			{
				return 0;
			}
			return WellbeingInternal;
		}
	}

	private bool ShouldBeUpdated
	{
		get
		{
			if (_updateEnabled)
			{
				return Enabled;
			}
			return false;
		}
	}

	private int WellbeingInternal
	{
		get
		{
			if (!IsFavorable)
			{
				return NeedSpec.GetUnfavorableWellbeing();
			}
			return NeedSpec.GetFavorableWellbeing();
		}
	}

	private float PointToMin => Points - NeedSpec.MinimumValue;

	public Need(NeedSpec needSpec, float deltaTimeInHours)
	{
		NeedSpec = needSpec;
		_isNeverPositive = NeedSpec.IsNeverPositive;
		IsCritical = NeedSpec.HasSpec<CriticalNeedSpec>();
		Reset();
		float num = NeedSpec.DailyDelta / 24f;
		_pointsWarningThreshold = Math.Abs(num) * NeedSpec.HoursWarningThreshold;
		_deltaPointsPerUpdate = num * deltaTimeInHours;
	}

	public void Update()
	{
		if (ShouldBeUpdated)
		{
			if (!_appliedEffectSinceLastUpdate)
			{
				AddPoints(_deltaPointsPerUpdate);
			}
			_appliedEffectSinceLastUpdate = false;
		}
	}

	public void Apply(in Effect effect)
	{
		if (TryRawAppraise(in effect, out var points))
		{
			AddPoints(points);
			_appliedEffectSinceLastUpdate = true;
		}
	}

	public bool TryAppraise(Effect effect, out float points)
	{
		if (TryRawAppraise(in effect, out var points2))
		{
			points = ApplyAttractiveness(Math.Min(points2, PointsToMax));
			return true;
		}
		points = 0f;
		return false;
	}

	public float EffectiveDurationInHours(ContinuousEffect effect)
	{
		float num = effect.PointsPerHour * NeedSpec.Effectiveness;
		return (PointsToMax - 0.01f) / num;
	}

	public void SetPoints(float points)
	{
		if (points < NeedSpec.MinimumValue)
		{
			Points = NeedSpec.MinimumValue;
		}
		else
		{
			Points = ((points > NeedSpec.MaximumValue) ? NeedSpec.MaximumValue : points);
		}
	}

	public void EnableUpdate()
	{
		_updateEnabled = true;
	}

	public void DisableUpdate()
	{
		_updateEnabled = false;
	}

	public void EnableNeed()
	{
		Enabled = true;
	}

	public void DisableNeed()
	{
		Enabled = false;
	}

	public void Reset()
	{
		SetPoints(NeedSpec.StartingValue);
	}

	private void AddPoints(float points)
	{
		SetPoints(Points + points);
	}

	private bool TryRawAppraise(in Effect effect, out float points)
	{
		if (!Enabled)
		{
			points = 0f;
			return true;
		}
		float num = effect.Points * NeedSpec.Effectiveness;
		int num2 = NonWastingEffectCount(num, effect.Count);
		if (num2 > 0)
		{
			points = num * (float)num2;
			return true;
		}
		points = 0f;
		return false;
	}

	private int NonWastingEffectCount(float effectPoints, int effectCount)
	{
		if (NeedSpec.Wastable)
		{
			return effectCount;
		}
		int val = Mathf.FloorToInt((effectPoints > 0f) ? (PointsToMax / effectPoints) : ((0f - PointToMin) / effectPoints));
		return Math.Min(effectCount, val);
	}

	private float ApplyAttractiveness(float points)
	{
		float num = 1f + PointsToMax * 0.1f;
		return points * NeedSpec.ImportanceMultiplier * num;
	}
}
