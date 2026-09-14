using System;
using Timberborn.NeedApplication;

namespace Timberborn.NeedApplicationUI;

internal static class ProbabilityDescriptionHelper
{
	private static readonly string LowProbabilityLocKey = "EffectProbability.Low";

	private static readonly string MediumProbabilityLocKey = "EffectProbability.Medium";

	private static readonly string HighProbabilityLocKey = "EffectProbability.High";

	public static string GetDisplayName(EffectProbability probability)
	{
		return probability switch
		{
			EffectProbability.Low => LowProbabilityLocKey, 
			EffectProbability.Medium => MediumProbabilityLocKey, 
			EffectProbability.High => HighProbabilityLocKey, 
			_ => throw new ArgumentOutOfRangeException($"Unknown probability: {probability}"), 
		};
	}
}
