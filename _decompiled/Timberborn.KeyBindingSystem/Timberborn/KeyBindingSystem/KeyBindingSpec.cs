using Timberborn.BlueprintSystem;

namespace Timberborn.KeyBindingSystem;

public record KeyBindingSpec : ComponentSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public string GroupId { get; init; }

	[Serialize]
	public string LocKey { get; init; }

	[Serialize]
	public int Order { get; init; }

	[Serialize]
	public bool AllowOtherModifiers { get; init; }

	[Serialize]
	public bool DevModeOnly { get; init; }
}
