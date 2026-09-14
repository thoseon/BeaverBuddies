using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.LocalizationSerialization;
using Timberborn.NeedSpecs;
using Timberborn.SpriteOperations;
using UnityEngine;

namespace Timberborn.Goods;

public record GoodSpec : ComponentSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public ImmutableArray<string> BackwardCompatibleIds { get; init; }

	[Serialize("DisplayNameLocKey")]
	public LocalizedText DisplayName { get; init; }

	[Serialize("PluralDisplayNameLocKey")]
	public LocalizedText PluralDisplayName { get; init; }

	[Serialize]
	public ImmutableArray<InstantEffectSpec> ConsumptionEffects { get; init; }

	[Serialize]
	public string GoodType { get; init; }

	[Serialize]
	public string StockpileVisualization { get; init; }

	[Serialize]
	public VisibleContainer VisibleContainer { get; init; }

	[Serialize]
	public Color ContainerColor { get; init; }

	[Serialize]
	public AssetRef<Material> ContainerMaterial { get; init; }

	[Serialize]
	public string CarryingAnimation { get; init; }

	[Serialize]
	public int Weight { get; init; }

	[Serialize]
	public string GoodGroupId { get; init; }

	[Serialize]
	public int GoodOrder { get; init; }

	[Serialize]
	public AssetRef<Sprite> Icon { get; init; }

	[Serialize("Icon")]
	public FlippedSprite IconFlipped { get; init; }

	[Serialize("Icon")]
	public UISprite IconSmall { get; init; }

	[Serialize]
	public bool ForceImport { get; init; }

	[Serialize]
	private string DisplayNameLocKey { get; init; }

	[Serialize]
	private string PluralDisplayNameLocKey { get; init; }

	public bool HasConsumptionEffects => !ConsumptionEffects.IsEmpty();

	public override string ToString()
	{
		return Id;
	}
}
