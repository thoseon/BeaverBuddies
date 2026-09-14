using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.UnityEngineSpecs;

public record CollidersSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<BoxColliderSpec> BoxColliders { get; init; }

	[Serialize]
	public ImmutableArray<SphereColliderSpec> SphereColliders { get; init; }

	[Serialize]
	public ImmutableArray<CapsuleColliderSpec> CapsuleColliders { get; init; }
}
