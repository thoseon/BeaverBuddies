using Timberborn.BlueprintSystem;
using Timberborn.LocalizationSerialization;
using UnityEngine;

namespace Timberborn.BonusSystem;

public record BonusTypeSpec : ComponentSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize("LocKey")]
	public LocalizedText DisplayName { get; init; }

	[Serialize]
	public float MinimumValue { get; init; }

	[Serialize]
	public float MaximumValue { get; init; }

	[Serialize]
	public AssetRef<Sprite> Icon { get; init; }

	[Serialize]
	private string LocKey { get; init; }
}
