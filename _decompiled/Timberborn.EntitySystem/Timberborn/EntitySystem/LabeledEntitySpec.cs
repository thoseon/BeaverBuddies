using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.EntitySystem;

public record LabeledEntitySpec : ComponentSpec
{
	[Serialize]
	public string DisplayNameLocKey { get; init; }

	[Serialize]
	public string DescriptionLocKey { get; init; }

	[Serialize]
	public string FlavorDescriptionLocKey { get; init; }

	[Serialize]
	public AssetRef<Sprite> Icon { get; init; }
}
