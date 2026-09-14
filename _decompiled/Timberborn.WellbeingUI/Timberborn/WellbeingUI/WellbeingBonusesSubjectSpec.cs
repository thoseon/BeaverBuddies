using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.WellbeingUI;

internal record WellbeingBonusesSubjectSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> Bonuses { get; init; }
}
