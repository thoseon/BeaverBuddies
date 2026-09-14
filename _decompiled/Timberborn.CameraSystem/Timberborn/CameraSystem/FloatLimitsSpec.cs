using Timberborn.BlueprintSystem;

namespace Timberborn.CameraSystem;

public record FloatLimitsSpec
{
	[Serialize]
	public float Min { get; init; }

	[Serialize]
	public float Max { get; init; }
}
