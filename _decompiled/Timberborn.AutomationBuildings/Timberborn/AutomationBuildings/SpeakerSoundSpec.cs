using Timberborn.BlueprintSystem;
using Timberborn.LocalizationSerialization;

namespace Timberborn.AutomationBuildings;

internal record SpeakerSoundSpec : ComponentSpec
{
	[Serialize]
	public string SoundId { get; init; }

	[Serialize("DisplayNameLocKey")]
	public LocalizedText DisplayName { get; init; }

	[Serialize]
	private string DisplayNameLocKey { get; init; }
}
