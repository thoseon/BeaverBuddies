using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.Particles;

internal record AnimationParticlesTriggerSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<AnimationParticle> AnimationParticles { get; init; }
}
