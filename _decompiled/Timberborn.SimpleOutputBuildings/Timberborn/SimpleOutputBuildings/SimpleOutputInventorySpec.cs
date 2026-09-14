using Timberborn.BlueprintSystem;

namespace Timberborn.SimpleOutputBuildings;

internal record SimpleOutputInventorySpec : ComponentSpec
{
	[Serialize]
	public int Capacity { get; init; }

	[Serialize]
	public bool IgnorableCapacity { get; init; }
}
