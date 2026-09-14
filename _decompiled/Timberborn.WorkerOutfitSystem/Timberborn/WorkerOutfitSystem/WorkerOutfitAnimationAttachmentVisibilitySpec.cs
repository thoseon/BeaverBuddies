using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.WorkerOutfitSystem;

internal record WorkerOutfitAnimationAttachmentVisibilitySpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<WorkerOutfitAnimationAttachmentSpec> WorkerOutfitAnimationAttachments { get; init; }
}
