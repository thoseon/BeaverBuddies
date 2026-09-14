using Timberborn.BlueprintSystem;

namespace Timberborn.EntitySystem;

public record SimpleLabeledEntitySpec : ComponentSpec
{
	[Serialize]
	public string EntityNameLocKey { get; init; }
}
