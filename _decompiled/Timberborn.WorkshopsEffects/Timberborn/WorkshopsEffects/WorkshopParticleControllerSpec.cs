using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.WorkshopsEffects;

internal record WorkshopParticleControllerSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> AttachmentIds { get; init; }
}
