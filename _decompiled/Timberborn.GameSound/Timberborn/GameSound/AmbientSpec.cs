using Timberborn.BlueprintSystem;

namespace Timberborn.GameSound;

internal record AmbientSpec : ComponentSpec
{
	[Serialize]
	public string DayAmbient { get; init; }

	[Serialize]
	public string NightAmbient { get; init; }

	[Serialize]
	public string WaterAmbient { get; init; }
}
