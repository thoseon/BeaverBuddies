using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.LocalizationSerialization;

namespace Timberborn.TutorialSystem;

internal record TutorialSpec : ComponentSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize("NameLocKey")]
	public LocalizedText DisplayName { get; init; }

	[Serialize]
	public string NameLocKey { get; init; }

	[Serialize]
	public ImmutableArray<string> RequiredTutorialIds { get; init; }

	[BackwardCompatible(2026, 1, 15, Compatibility.Save)]
	[Serialize]
	public string SkipIfTutorialFinished { get; init; }

	[Serialize]
	public int SortOrder { get; init; }

	[Serialize]
	public ImmutableArray<string> Stages { get; init; }
}
