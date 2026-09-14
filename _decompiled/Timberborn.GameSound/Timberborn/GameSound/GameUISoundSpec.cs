using Timberborn.BlueprintSystem;

namespace Timberborn.GameSound;

internal record GameUISoundSpec : ComponentSpec
{
	[Serialize]
	public string WellbeingHighscore { get; init; }

	[Serialize]
	public string FieldPlaced { get; init; }

	[Serialize]
	public string BlinkingSoundKey { get; init; }

	[Serialize]
	public string BadtideStartedSoundKey { get; init; }

	[Serialize]
	public string DroughtStartedSoundKey { get; init; }

	[Serialize]
	public string TemperateWeatherStartedSoundKey { get; init; }

	[Serialize]
	public string WonderCongratulationSoundKey { get; init; }
}
