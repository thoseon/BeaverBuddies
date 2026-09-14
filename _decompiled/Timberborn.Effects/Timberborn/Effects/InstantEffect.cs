using Timberborn.NeedSpecs;

namespace Timberborn.Effects;

public readonly struct InstantEffect
{
	public string NeedId { get; }

	public float Points { get; }

	public int Count { get; }

	public InstantEffect(string needId, float points, int count)
	{
		NeedId = needId;
		Points = points;
		Count = count;
	}

	public static InstantEffect FromSpec(InstantEffectSpec instantEffectSpec, int count)
	{
		return new InstantEffect(instantEffectSpec.NeedId, instantEffectSpec.Points, count);
	}

	public static InstantEffect DiscretizeContinuousEffect(string needId)
	{
		return new InstantEffect(needId, 0.05f, 20);
	}
}
