using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.EnterableSystem;

internal record EnterableParticleControllerSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> AttachmentIds { get; init; }
}
