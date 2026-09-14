using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.PowerGenerationUI;

internal record PowerGeneratorParticleControllerSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> AttachmentIds { get; init; }
}
