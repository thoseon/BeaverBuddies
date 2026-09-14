using Timberborn.BlueprintSystem;

namespace Timberborn.BlockObjectModelSystem;

internal record BlockObjectModelSpec : ComponentSpec
{
	[Serialize]
	public string FullModelName { get; init; }

	[Serialize]
	public string UncoveredModelName { get; init; }

	[Serialize]
	public string UndergroundModelName { get; init; }

	[Serialize]
	public int UndergroundModelDepth { get; init; }
}
