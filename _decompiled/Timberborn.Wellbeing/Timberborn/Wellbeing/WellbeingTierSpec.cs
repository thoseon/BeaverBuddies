using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.Wellbeing;

internal record WellbeingTierSpec : ComponentSpec
{
	[Serialize]
	public string CharacterType { get; init; }

	[Serialize]
	public string BonusId { get; init; }

	[Serialize]
	public ImmutableArray<WellbeingTierBonusSpec> Bonuses { get; init; }

	[Serialize]
	public int WellbeingThreshold { get; init; }

	[Serialize]
	public float MultiplierIncrement { get; init; }
}
