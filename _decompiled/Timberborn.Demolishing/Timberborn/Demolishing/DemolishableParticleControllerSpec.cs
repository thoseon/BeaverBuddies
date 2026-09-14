using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.Demolishing;

internal record DemolishableParticleControllerSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<DemolishableParticle> DemolishableParticles { get; init; }
}
