using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.SkySystem;

internal record SunSpec : ComponentSpec
{
	[Serialize]
	public AssetRef<Light> SunPrefab { get; init; }

	[Serialize]
	public DayStageColorsSpec SunriseColors { get; init; }

	[Serialize]
	public DayStageColorsSpec DayColors { get; init; }

	[Serialize]
	public DayStageColorsSpec SunsetColors { get; init; }

	[Serialize]
	public DayStageColorsSpec NightColors { get; init; }

	[Serialize]
	public float RotateWithCameraOffset { get; init; }
}
