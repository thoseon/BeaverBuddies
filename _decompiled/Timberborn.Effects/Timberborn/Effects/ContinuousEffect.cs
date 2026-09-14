using System;
using Timberborn.NeedSpecs;

namespace Timberborn.Effects;

public readonly struct ContinuousEffect : IEquatable<ContinuousEffect>
{
	public string NeedId { get; }

	public float PointsPerHour { get; }

	public ContinuousEffect(string needId, float pointsPerHour)
	{
		NeedId = needId;
		PointsPerHour = pointsPerHour;
	}

	public static ContinuousEffect FromSpec(ContinuousEffectSpec continuousEffectSpec)
	{
		return new ContinuousEffect(continuousEffectSpec.NeedId, continuousEffectSpec.PointsPerHour);
	}

	public bool Equals(ContinuousEffect other)
	{
		if (NeedId == other.NeedId)
		{
			return PointsPerHour.Equals(other.PointsPerHour);
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is ContinuousEffect other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(NeedId, PointsPerHour);
	}

	public static bool operator ==(ContinuousEffect left, ContinuousEffect right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ContinuousEffect left, ContinuousEffect right)
	{
		return !left.Equals(right);
	}
}
