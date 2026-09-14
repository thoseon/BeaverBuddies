using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.LocalizationSerialization;
using UnityEngine;

namespace Timberborn.FactionSystem;

public record FactionSpec : ComponentSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public int Order { get; init; }

	[Serialize("DisplayNameLocKey")]
	public LocalizedText DisplayName { get; init; }

	[Serialize("DescriptionLocKey")]
	public LocalizedText Description { get; init; }

	[Serialize]
	public AssetRef<Sprite> Avatar { get; init; }

	[Serialize]
	public AssetRef<Sprite> ChildAvatar { get; init; }

	[Serialize]
	public AssetRef<Sprite> BotAvatar { get; init; }

	[Serialize]
	public AssetRef<Sprite> ContaminatedAdultAvatar { get; init; }

	[Serialize]
	public AssetRef<Sprite> ContaminatedChildAvatar { get; init; }

	[Serialize]
	public AssetRef<Sprite> Logo { get; init; }

	[Serialize]
	public AssetRef<Sprite> NewGameFullAvatar { get; init; }

	[Serialize]
	public ImmutableArray<AssetRef<Texture2D>> Textures { get; init; }

	[Serialize]
	public ImmutableArray<AssetRef<Texture2D>> ChildTextures { get; init; }

	[Serialize]
	public ImmutableArray<string> MaterialCollectionIds { get; init; }

	[Serialize]
	public ImmutableArray<string> TemplateCollectionIds { get; init; }

	[Serialize]
	public AssetRef<Material> PathMaterial { get; init; }

	[Serialize]
	public AssetRef<Material> BaseWoodMaterial { get; init; }

	[Serialize]
	public ImmutableArray<string> NeedCollectionIds { get; init; }

	[Serialize]
	public ImmutableArray<string> GoodCollectionIds { get; init; }

	[Serialize]
	public ImmutableArray<BlueprintModifierSpec> BlueprintModifiers { get; init; }

	[Serialize]
	public string StartingBuildingId { get; init; }

	[Serialize]
	public string SoundId { get; init; }

	[Serialize("GameOverMessageLocKey")]
	public LocalizedText GameOverMessage { get; init; }

	[Serialize("GameOverFlavorLocKey")]
	public LocalizedText GameOverFlavor { get; init; }

	[Serialize]
	private string DisplayNameLocKey { get; init; }

	[Serialize]
	private string DescriptionLocKey { get; init; }

	[Serialize]
	private string GameOverMessageLocKey { get; init; }

	[Serialize]
	private string GameOverFlavorLocKey { get; init; }
}
