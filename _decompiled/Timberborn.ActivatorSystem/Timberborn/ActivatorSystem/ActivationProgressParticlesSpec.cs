using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.ActivatorSystem;

internal record ActivationProgressParticlesSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> AttachmentIds { get; init; }

	[Serialize]
	public int MinEmission { get; init; }

	[Serialize]
	public int MaxEmission { get; init; }
}
