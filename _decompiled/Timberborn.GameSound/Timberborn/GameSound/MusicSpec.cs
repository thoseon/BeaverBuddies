using Timberborn.BlueprintSystem;

namespace Timberborn.GameSound;

internal record MusicSpec : ComponentSpec
{
	[Serialize]
	public string DroughtTrack { get; init; }

	[Serialize]
	public string StandardTrack { get; init; }

	[Serialize]
	public string StandardPhrase { get; init; }

	[Serialize]
	public float MinDelay { get; init; }

	[Serialize]
	public float MaxDelay { get; init; }
}
