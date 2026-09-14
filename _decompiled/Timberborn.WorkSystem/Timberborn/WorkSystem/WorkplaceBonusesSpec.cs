using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.BonusSystem;

namespace Timberborn.WorkSystem;

internal record WorkplaceBonusesSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<BonusSpec> WorkerBonuses { get; init; }
}
