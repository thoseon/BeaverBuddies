using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.SkySystem;

internal record SkyboxPositionerSpec : ComponentSpec
{
	[Serialize]
	public AssetRef<Material> Skybox { get; init; }

	[Serialize]
	public float DayProgressSunrise { get; init; }

	[Serialize]
	public float DayProgressDay { get; init; }

	[Serialize]
	public float DayProgressSunset { get; init; }

	[Serialize]
	public float DayProgressNight { get; init; }
}
