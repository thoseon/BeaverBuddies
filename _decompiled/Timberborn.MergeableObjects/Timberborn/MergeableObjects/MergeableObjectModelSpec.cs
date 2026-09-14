using Timberborn.BlueprintSystem;

namespace Timberborn.MergeableObjects;

internal record MergeableObjectModelSpec : ComponentSpec
{
	[Serialize]
	public string ModelNamePrefix { get; init; }
}
