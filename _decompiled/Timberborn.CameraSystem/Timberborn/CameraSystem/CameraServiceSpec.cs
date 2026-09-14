using Timberborn.BlueprintSystem;

namespace Timberborn.CameraSystem;

internal record CameraServiceSpec : ComponentSpec
{
	[Serialize]
	public float HorizontalAngle { get; init; }

	[Serialize]
	public float VerticalAngle { get; init; }

	[Serialize]
	public FloatLimitsSpec VerticalAngleLimits { get; init; }

	[Serialize]
	public float ZoomLevel { get; init; }

	[Serialize]
	public float ZoomSpeed { get; init; }

	[Serialize]
	public float ZoomBase { get; init; }

	[Serialize]
	public float BaseDistance { get; init; }

	[Serialize]
	public FloatLimitsSpec DefaultZoomLimits { get; init; }

	[Serialize]
	public FloatLimitsSpec UnlockedZoomLimits { get; init; }

	[Serialize]
	public FloatLimitsSpec MapEditorZoomLimits { get; init; }

	[Serialize]
	public FloatLimitsSpec FreeModeZoomLimits { get; init; }

	[Serialize]
	public float MapMargin { get; init; }

	[Serialize]
	public float FreeModeMapMargin { get; init; }
}
