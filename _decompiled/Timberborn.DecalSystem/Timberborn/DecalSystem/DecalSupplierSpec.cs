using Timberborn.BlueprintSystem;

namespace Timberborn.DecalSystem;

internal record DecalSupplierSpec : ComponentSpec
{
	[Serialize]
	public string Category { get; init; }
}
