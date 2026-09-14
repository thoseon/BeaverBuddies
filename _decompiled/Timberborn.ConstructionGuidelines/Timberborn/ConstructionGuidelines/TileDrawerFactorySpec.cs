using Timberborn.BlueprintSystem;

namespace Timberborn.ConstructionGuidelines;

internal record TileDrawerFactorySpec : ComponentSpec
{
	[Serialize]
	public string MeshResourcePath { get; init; }

	[Serialize]
	public string TilesOnSameLevelMaterialResourcePath { get; init; }

	[Serialize]
	public string TilesBelowMaterialResourcePath { get; init; }

	[Serialize]
	public string TilesAboveMaterialResourcePath { get; init; }

	[Serialize]
	public string FootprintTilesMaterialResourcePath { get; init; }
}
