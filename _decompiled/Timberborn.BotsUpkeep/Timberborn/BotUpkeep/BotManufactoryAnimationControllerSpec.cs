using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.BotUpkeep;

internal record BotManufactoryAnimationControllerSpec : ComponentSpec
{
	[Serialize]
	public float AssemblyDuration { get; init; }

	[Serialize]
	public float RingRotationSpeed { get; init; }

	[Serialize]
	public float DrillRotationSpeed { get; init; }

	[Serialize]
	public string RingName { get; init; }

	[Serialize]
	public string DrillName { get; init; }

	[Serialize]
	public ImmutableArray<string> AttachmentIds { get; init; }
}
