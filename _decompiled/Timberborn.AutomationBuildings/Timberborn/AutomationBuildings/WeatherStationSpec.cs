using Timberborn.BlueprintSystem;

namespace Timberborn.AutomationBuildings;

internal record WeatherStationSpec : ComponentSpec
{
	[Serialize]
	public int MaxEarlyActivationHours { get; init; }
}
