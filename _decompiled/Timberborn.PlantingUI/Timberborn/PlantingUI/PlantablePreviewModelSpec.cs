using Timberborn.AssetSystem;
using Timberborn.BlueprintSystem;

namespace Timberborn.PlantingUI;

internal record PlantablePreviewModelSpec : ComponentSpec
{
	[Serialize]
	public AssetRef<BinaryData> Model { get; init; }
}
