using Timberborn.BlueprintSystem;

namespace Timberborn.SkySystem;

internal record HazardousWeatherFogSettingsSpec
{
	[Serialize]
	public string HazardousWeatherId { get; init; }

	[Serialize]
	public FogSettingsSpec FogSettings { get; init; }
}
