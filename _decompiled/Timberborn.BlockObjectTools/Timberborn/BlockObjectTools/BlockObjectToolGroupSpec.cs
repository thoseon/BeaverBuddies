using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.BlockObjectTools;

public record BlockObjectToolGroupSpec : ComponentSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public int Order { get; init; }

	[Serialize]
	public string NameLocKey { get; init; }

	[Serialize]
	public AssetRef<Sprite> Icon { get; init; }

	[Serialize]
	public bool FallbackGroup { get; init; }
}
