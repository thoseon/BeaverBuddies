using Timberborn.BlueprintSystem;
using Timberborn.LocalizationSerialization;

namespace Timberborn.TutorialSystem;

internal record TutorialStageSpec : ComponentSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize("IntroLocKey")]
	public LocalizedText Intro { get; init; }

	[Serialize]
	public string IntroLocKey { get; init; }
}
