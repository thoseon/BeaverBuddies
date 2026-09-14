using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.LocalizationSerialization;

namespace Timberborn.WorkerTypes;

public record WorkerTypeSpec : ComponentSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public ImmutableArray<string> BackwardCompatibleIds { get; init; }

	[Serialize("DisplayNameLocKey")]
	public LocalizedText DisplayName { get; init; }

	[Serialize("WorkerOnlyTextLocKey")]
	public LocalizedText WorkerOnlyText { get; init; }

	[Serialize]
	public bool IgnoresWorkingHours { get; init; }

	[Serialize]
	private string DisplayNameLocKey { get; init; }

	[Serialize]
	private string WorkerOnlyTextLocKey { get; init; }
}
