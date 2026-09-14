using Timberborn.BlueprintSystem;

namespace Timberborn.Planting;

public record PlanterBuildingSpec : ComponentSpec
{
	[Serialize]
	public string PlantableResourceGroup { get; init; }
}
