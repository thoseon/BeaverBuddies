using Timberborn.BlueprintSystem;

namespace Timberborn.BotsUI;

public record BotSelectionSoundSpec : ComponentSpec
{
	[Serialize]
	public string SoundNameKey { get; init; }
}
