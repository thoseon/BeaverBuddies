using Timberborn.BlueprintSystem;

namespace Timberborn.CameraSystem;

internal record KeyboardCameraControllerSpec : ComponentSpec
{
	[Serialize]
	public int JumpRotationAngle { get; init; }

	[Serialize]
	public int JumpRotationSpeedInAnglePerUpdate { get; init; }

	[Serialize]
	public float BaseZoomSpeed { get; init; }
}
