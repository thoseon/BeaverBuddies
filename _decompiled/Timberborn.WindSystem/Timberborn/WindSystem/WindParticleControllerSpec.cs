using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.WindSystem;

internal record WindParticleControllerSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> AttachmentIds { get; init; }
}
