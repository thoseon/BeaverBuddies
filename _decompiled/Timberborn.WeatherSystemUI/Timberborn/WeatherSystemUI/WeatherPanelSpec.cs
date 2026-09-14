using Timberborn.BlueprintSystem;

namespace Timberborn.WeatherSystemUI;

internal record WeatherPanelSpec : ComponentSpec
{
	[Serialize]
	public int NumberOfBlinks { get; init; }

	[Serialize]
	public float SecondsBetweenBlinks { get; init; }
}
