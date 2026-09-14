using Timberborn.BlueprintSystem;

namespace Timberborn.Explosions;

internal record UnstableCoreEffectsSpawnerSpec : ComponentSpec
{
	[Serialize]
	public string ExplosionPrefabPath { get; init; }
}
