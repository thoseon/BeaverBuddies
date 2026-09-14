using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.BlockObjectTools;

internal record BlockObjectDeletionToolSpec : ComponentSpec
{
	[Serialize]
	public Color DeletedObjectHighlightColor { get; init; }

	[Serialize]
	public Color DeletedAreaTileColor { get; init; }

	[Serialize]
	public Color DeletedAreaSideColor { get; init; }
}
