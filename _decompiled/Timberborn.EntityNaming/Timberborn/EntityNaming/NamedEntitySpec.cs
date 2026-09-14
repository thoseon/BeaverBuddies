using Timberborn.BlueprintSystem;

namespace Timberborn.EntityNaming;

internal record NamedEntitySpec : ComponentSpec
{
	[Serialize]
	public bool IsEditable { get; init; }
}
