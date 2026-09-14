using Timberborn.BlueprintSystem;

namespace Timberborn.AutomationBuildings;

internal record TimerModelSpec : ComponentSpec
{
	[Serialize]
	public string ProgressObjectName { get; init; }

	[Serialize]
	public float MinHeight { get; init; }

	[Serialize]
	public float MaxHeight { get; init; }
}
