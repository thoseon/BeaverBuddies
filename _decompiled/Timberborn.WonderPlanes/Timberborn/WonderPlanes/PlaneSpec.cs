using Timberborn.BlueprintSystem;

namespace Timberborn.WonderPlanes;

internal record PlaneSpec : ComponentSpec
{
	[Serialize]
	public string PilotSeatName { get; init; }

	[Serialize]
	public float RotationSpeed { get; init; }
}
