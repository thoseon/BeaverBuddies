using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.WaterSourceSystem;

public record HazardousWeatherWaterSourceSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> ActiveInHazardousWeather { get; init; }
}
