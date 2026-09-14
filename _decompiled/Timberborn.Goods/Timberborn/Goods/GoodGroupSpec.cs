using Timberborn.BlueprintSystem;
using Timberborn.LocalizationSerialization;
using UnityEngine;

namespace Timberborn.Goods;

public record GoodGroupSpec : ComponentSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public int Order { get; init; }

	[Serialize("DisplayNameLocKey")]
	public LocalizedText DisplayName { get; init; }

	[Serialize]
	public AssetRef<Sprite> Icon { get; init; }

	[Serialize]
	public bool SingleResourceGroup { get; init; }

	[Serialize]
	private string DisplayNameLocKey { get; init; }
}
