using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.BlockingSystem;

internal record BlockableObjectParticleControllerSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> AttachmentIds { get; init; }
}
