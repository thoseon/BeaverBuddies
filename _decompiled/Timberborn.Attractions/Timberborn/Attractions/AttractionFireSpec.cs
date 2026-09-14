using Timberborn.BlueprintSystem;

namespace Timberborn.Attractions;

internal record AttractionFireSpec : ComponentSpec
{
	[Serialize]
	public string WoodstackName { get; init; }
}
