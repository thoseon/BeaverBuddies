using Timberborn.BlueprintSystem;

namespace Timberborn.SoilMoistureSystem;

internal record DryObjectModelSpec : ComponentSpec
{
	[Serialize]
	public string WetModelName { get; init; }

	[Serialize]
	public string DryModelName { get; init; }
}
