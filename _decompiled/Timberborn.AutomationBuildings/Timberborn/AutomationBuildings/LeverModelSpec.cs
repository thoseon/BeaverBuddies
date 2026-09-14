using Timberborn.BlueprintSystem;

namespace Timberborn.AutomationBuildings;

internal record LeverModelSpec : ComponentSpec
{
	[Serialize]
	public string OnModelName { get; init; }

	[Serialize]
	public string OffModelName { get; init; }
}
