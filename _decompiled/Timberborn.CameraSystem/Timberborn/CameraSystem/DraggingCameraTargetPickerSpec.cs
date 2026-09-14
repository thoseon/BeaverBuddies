using Timberborn.BlueprintSystem;

namespace Timberborn.CameraSystem;

internal record DraggingCameraTargetPickerSpec : ComponentSpec
{
	[Serialize]
	public float MovementSpeed { get; init; }
}
