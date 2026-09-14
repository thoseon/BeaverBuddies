using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.MechanicalSystem;

internal record MechanicalNodeParticlesControllerSpec : ComponentSpec
{
	[Serialize]
	public float MinEfficiency { get; init; }

	[Serialize]
	public ImmutableArray<string> AttachmentIds { get; init; }
}
