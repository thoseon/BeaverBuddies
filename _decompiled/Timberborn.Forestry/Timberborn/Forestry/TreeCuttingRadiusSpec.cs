using Timberborn.BlueprintSystem;

namespace Timberborn.Forestry;

internal record TreeCuttingRadiusSpec : ComponentSpec
{
	[Serialize]
	public float Radius { get; init; }
}
