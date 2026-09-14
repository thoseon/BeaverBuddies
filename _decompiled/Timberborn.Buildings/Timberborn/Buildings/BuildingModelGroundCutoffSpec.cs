using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.Buildings;

internal record BuildingModelGroundCutoffSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> Targets { get; init; }

	[Serialize]
	public float Offset { get; init; }
}
