using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.Illumination;

public record IlluminatorLightObjectsSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> AttachmentIds { get; init; }
}
