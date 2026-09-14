using Timberborn.BlueprintSystem;

namespace Timberborn.PathSystem;

internal record DynamicPathModelSpec : ComponentSpec
{
	[Serialize]
	public string GroundModelPrefix { get; init; }

	[Serialize]
	public string RoofModelPrefix { get; init; }
}
