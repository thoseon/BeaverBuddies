using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.BlockSystemNavigation;

internal record BlockObjectNavMeshSettingsSpec : ComponentSpec
{
	[Serialize]
	public bool NoAutoWalls { get; init; }

	[Serialize]
	public bool GenerateFloorsOnStackable { get; init; }

	[Serialize]
	public ImmutableArray<BlockObjectNavMeshEdgeGroupSpec> EdgeGroups { get; init; }

	[Serialize]
	public ImmutableArray<BlockObjectNavMeshUnblockedCoordinatesSpec> UnblockedCoordinates { get; init; }

	[Serialize]
	public ImmutableArray<BlockObjectNavMeshBlockedEdgeSpec> BlockedEdges { get; init; }
}
