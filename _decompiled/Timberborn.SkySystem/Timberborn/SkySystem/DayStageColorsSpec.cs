using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.SkySystem;

internal record DayStageColorsSpec
{
	[Serialize]
	public Color SunColor { get; init; }

	[Serialize]
	public float SunIntensity { get; init; }

	[Serialize]
	public float SunXAngle { get; init; }

	[Serialize]
	public float ShadowStrength { get; init; }

	[Serialize]
	public Color AmbientSkyColor { get; init; }

	[Serialize]
	public Color AmbientEquatorColor { get; init; }

	[Serialize]
	public Color AmbientGroundColor { get; init; }

	[Serialize]
	public float ReflectionsIntensity { get; init; }

	[Serialize]
	public FogSettingsSpec TemperateWeatherFog { get; init; }

	[Serialize]
	public ImmutableArray<HazardousWeatherFogSettingsSpec> HazardousWeatherFogs { get; init; }
}
