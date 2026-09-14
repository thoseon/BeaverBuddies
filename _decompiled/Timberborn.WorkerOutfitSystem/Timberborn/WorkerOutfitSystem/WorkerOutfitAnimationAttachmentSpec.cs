using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.WorkerOutfitSystem;

internal record WorkerOutfitAnimationAttachmentSpec
{
	[Serialize]
	public string WorkerOutfit { get; init; }

	[Serialize]
	public ImmutableArray<string> AnimationNames { get; init; }

	[Serialize]
	public ImmutableArray<string> ShowWhenActive { get; init; }

	[Serialize]
	public ImmutableArray<string> HideWhenActive { get; init; }
}
