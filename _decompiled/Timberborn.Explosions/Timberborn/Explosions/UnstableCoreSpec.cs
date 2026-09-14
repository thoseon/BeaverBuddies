using Timberborn.BlueprintSystem;

namespace Timberborn.Explosions;

internal record UnstableCoreSpec : ComponentSpec
{
	[Serialize]
	public int MinExplosionRadius { get; init; }

	[Serialize]
	public int MaxExplosionRadius { get; init; }

	[Serialize]
	public int DefaultExplosionRadius { get; init; }

	[Serialize]
	public float InnerRadius { get; init; }
}
