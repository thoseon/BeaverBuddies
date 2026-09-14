using Timberborn.BlueprintSystem;

namespace Timberborn.Explosions;

public record UnstableCoreLightingSpec : ComponentSpec
{
	[Serialize]
	public float MinInterval { get; init; }

	[Serialize]
	public float MaxInterval { get; init; }
}
