using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.Wonders;

internal record WonderParticleControllerSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> AttachmentIds { get; init; }
}
