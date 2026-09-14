using Timberborn.BlueprintSystem;

namespace Timberborn.DecalSystem;

internal record DecalSupplierBuildingIconSpec : ComponentSpec
{
	[Serialize]
	public string IconRendererName { get; init; }
}
