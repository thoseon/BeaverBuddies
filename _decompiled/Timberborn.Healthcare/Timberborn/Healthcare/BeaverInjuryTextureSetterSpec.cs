using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.Healthcare;

internal record BeaverInjuryTextureSetterSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<BeaverInjuryTextureSet> InjuryTextureSets { get; init; }
}
