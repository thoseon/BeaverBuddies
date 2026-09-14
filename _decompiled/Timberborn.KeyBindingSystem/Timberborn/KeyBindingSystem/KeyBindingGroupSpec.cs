using Timberborn.BlueprintSystem;
using Timberborn.LocalizationSerialization;

namespace Timberborn.KeyBindingSystem;

public record KeyBindingGroupSpec : ComponentSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public int Order { get; init; }

	[Serialize("LocKey")]
	public LocalizedText DisplayName { get; init; }

	[Serialize]
	private string LocKey { get; init; }

	public bool IsHiddenGroup => HasSpec<HiddenKeyBindingGroupSpec>();
}
