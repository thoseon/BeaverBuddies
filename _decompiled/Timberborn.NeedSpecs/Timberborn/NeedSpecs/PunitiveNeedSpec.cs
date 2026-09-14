using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.BonusSystem;

namespace Timberborn.NeedSpecs;

public record PunitiveNeedSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<BonusSpec> Penalties { get; init; }
}
