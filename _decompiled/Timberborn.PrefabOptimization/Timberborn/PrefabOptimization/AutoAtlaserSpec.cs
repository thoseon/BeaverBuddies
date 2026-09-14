using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.PrefabOptimization;

internal record AutoAtlaserSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<AutoAtlasSpec> AutoAtlases { get; init; }
}
