using Timberborn.BlueprintSystem;

namespace Timberborn.TubeSystem;

internal record TubeModelSpec : ComponentSpec
{
	[Serialize]
	public string ModelPrefix { get; init; }
}
