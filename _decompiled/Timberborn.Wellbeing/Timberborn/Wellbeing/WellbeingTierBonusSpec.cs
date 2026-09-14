using Timberborn.BlueprintSystem;

namespace Timberborn.Wellbeing;

internal record WellbeingTierBonusSpec
{
	[Serialize]
	public int Wellbeing { get; init; }

	[Serialize]
	public float Multiplier { get; init; }
}
