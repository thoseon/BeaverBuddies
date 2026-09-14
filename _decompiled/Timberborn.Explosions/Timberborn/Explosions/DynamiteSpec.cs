using Timberborn.BlueprintSystem;

namespace Timberborn.Explosions;

internal record DynamiteSpec : ComponentSpec
{
	[Serialize]
	public int Depth { get; init; }

	[Serialize]
	public string ExplosionPrefabPath { get; init; }
}
