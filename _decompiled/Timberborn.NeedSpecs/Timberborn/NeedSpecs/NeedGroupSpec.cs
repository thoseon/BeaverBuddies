using Timberborn.BlueprintSystem;
using Timberborn.LocalizationSerialization;

namespace Timberborn.NeedSpecs;

public record NeedGroupSpec : ComponentSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public int Order { get; init; }

	[Serialize("DisplayNameLocKey")]
	public LocalizedText DisplayName { get; init; }

	[Serialize]
	private string DisplayNameLocKey { get; init; }
}
