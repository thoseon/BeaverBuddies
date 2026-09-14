using Timberborn.BlueprintSystem;

namespace Timberborn.NaturalResourcesReproduction;

internal record ReproducibleSpec : ComponentSpec
{
	[Serialize]
	public float ReproductionChance { get; init; }
}
