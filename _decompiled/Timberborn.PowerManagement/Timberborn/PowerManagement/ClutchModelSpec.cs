using Timberborn.BlueprintSystem;

namespace Timberborn.PowerManagement;

internal record ClutchModelSpec : ComponentSpec
{
	[Serialize]
	public string EngagedModelName { get; init; }

	[Serialize]
	public string DisengagedModelName { get; init; }
}
