using Timberborn.BlueprintSystem;

namespace Timberborn.Terraforming;

public record DrillScrewRotatorSpec : ComponentSpec
{
	[Serialize]
	public float MinimumRotationSpeed { get; init; }

	[Serialize]
	public float RotationSpeedPerWorker { get; init; }
}
