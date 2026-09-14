using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.Attractions;

internal record GoodConsumingAttractionSurfaceControllerSpec : ComponentSpec
{
	[Serialize]
	public string SurfaceName { get; init; }

	[Serialize]
	public ImmutableArray<string> AttachmentIds { get; init; }
}
