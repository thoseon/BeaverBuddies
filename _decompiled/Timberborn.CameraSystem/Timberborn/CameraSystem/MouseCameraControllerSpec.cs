using Timberborn.BlueprintSystem;

namespace Timberborn.CameraSystem;

internal record MouseCameraControllerSpec : ComponentSpec
{
	[Serialize]
	public float RmbRotationSpeed { get; init; }

	[Serialize]
	public float RmbRotationMinDistance { get; init; }
}
