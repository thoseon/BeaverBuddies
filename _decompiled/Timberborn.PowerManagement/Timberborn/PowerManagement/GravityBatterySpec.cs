using Timberborn.BlueprintSystem;

namespace Timberborn.PowerManagement;

internal record GravityBatterySpec : ComponentSpec
{
	[Serialize]
	public int CapacityPerTile { get; init; }
}
