using Timberborn.BlueprintSystem;

namespace Timberborn.NeedBehaviorSystem;

internal record CriticalNeedStateAnimationSpec : ComponentSpec
{
	[Serialize]
	public string NeedId { get; init; }

	[Serialize]
	public string Animation { get; init; }
}
