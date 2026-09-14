using Timberborn.BlueprintSystem;

namespace Timberborn.AutomationBuildings;

internal record GateModelSpec : ComponentSpec
{
	[Serialize]
	public string OpenModelName { get; init; }

	[Serialize]
	public string ClosedModelName { get; init; }
}
