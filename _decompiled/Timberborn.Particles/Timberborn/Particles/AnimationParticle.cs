using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.Particles;

internal record AnimationParticle
{
	[Serialize]
	public string AnimationName { get; init; }

	[Serialize]
	public string ParticlesAttachmentId { get; init; }

	[Serialize]
	public ImmutableArray<float> TriggerTimes { get; init; }
}
