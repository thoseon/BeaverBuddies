using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.BlockSystemNavigation;

internal record BlockObjectNavMeshEdgeGroupSpec
{
	[Serialize]
	public float Cost { get; init; }

	[Serialize]
	public BlockObjectNavMeshGroup Group { get; init; }

	[Serialize]
	public bool IsPath { get; init; }

	[Serialize]
	public ImmutableArray<BlockObjectNavMeshEdgeSpec> AddedEdges { get; init; }

	public bool UseGroup => Group.UseGroup;

	public string GroupName => Group.GroupName;
}
