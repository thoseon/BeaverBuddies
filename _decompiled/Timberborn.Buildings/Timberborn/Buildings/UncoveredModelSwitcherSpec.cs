using Timberborn.BlueprintSystem;

namespace Timberborn.Buildings;

internal record UncoveredModelSwitcherSpec : ComponentSpec
{
	[Serialize]
	public string FullModelName { get; init; }

	[Serialize]
	public string UncoveredModelName { get; init; }
}
