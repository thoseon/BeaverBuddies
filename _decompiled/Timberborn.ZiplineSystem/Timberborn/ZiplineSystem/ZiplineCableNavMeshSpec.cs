using Timberborn.BlueprintSystem;

namespace Timberborn.ZiplineSystem;

public record ZiplineCableNavMeshSpec : ComponentSpec
{
	[Serialize]
	public float CableUnitCost { get; init; }
}
