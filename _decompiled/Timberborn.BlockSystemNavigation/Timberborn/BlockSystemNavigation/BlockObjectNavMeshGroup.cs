using Timberborn.BlueprintSystem;

namespace Timberborn.BlockSystemNavigation;

internal record BlockObjectNavMeshGroup
{
	[Serialize]
	public bool UseGroup { get; init; }

	[Serialize]
	public string GroupName { get; init; }
}
