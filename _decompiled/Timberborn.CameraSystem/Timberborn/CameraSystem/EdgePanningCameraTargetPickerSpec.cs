using Timberborn.BlueprintSystem;

namespace Timberborn.CameraSystem;

internal record EdgePanningCameraTargetPickerSpec : ComponentSpec
{
	[Serialize]
	public float MinBaseSpeed { get; init; }

	[Serialize]
	public float MaxBaseSpeed { get; init; }

	[Serialize]
	public float FastMovementSpeedBonus { get; init; }
}
