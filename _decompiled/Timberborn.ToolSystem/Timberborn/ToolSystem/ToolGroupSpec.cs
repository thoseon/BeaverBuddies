using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.ToolSystem;

public record ToolGroupSpec : ComponentSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public string DisplayNameLocKey { get; init; }

	[Serialize]
	public AssetRef<Sprite> Icon { get; init; }
}
