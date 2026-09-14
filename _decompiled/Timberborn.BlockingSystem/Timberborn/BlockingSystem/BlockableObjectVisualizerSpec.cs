using Timberborn.BlueprintSystem;

namespace Timberborn.BlockingSystem;

internal record BlockableObjectVisualizerSpec : ComponentSpec
{
	[Serialize]
	public string HideableObjectName { get; init; }
}
