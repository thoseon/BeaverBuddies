using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.WaterBuildings;

internal record WaterMoverParticleControllerSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> AttachmentIds { get; init; }
}
