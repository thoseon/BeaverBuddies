using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.BlockObjectTools;

internal record PreviewShowerSpec : ComponentSpec
{
	[Serialize]
	public Color BuildablePreview { get; init; }

	[Serialize]
	public Color UnbuildablePreview { get; init; }

	[Serialize]
	public Color WarningPreview { get; init; }
}
