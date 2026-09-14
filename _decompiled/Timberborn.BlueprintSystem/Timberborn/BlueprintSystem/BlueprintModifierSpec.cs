namespace Timberborn.BlueprintSystem;

public record BlueprintModifierSpec
{
	[Serialize]
	public AssetRef<BlueprintAsset> Original { get; init; }

	[Serialize]
	public AssetRef<BlueprintAsset> Modifier { get; init; }
}
