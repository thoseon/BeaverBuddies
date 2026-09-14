using Timberborn.BlueprintSystem;

namespace Timberborn.CoreSound;

internal record BasicSelectionSoundSpec : ComponentSpec
{
	[Serialize]
	public string SoundName { get; init; }

	[Serialize]
	public string AlternativeSoundName { get; init; }
}
