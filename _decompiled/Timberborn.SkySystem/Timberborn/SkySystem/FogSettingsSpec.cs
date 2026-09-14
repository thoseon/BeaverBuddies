using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.SkySystem;

internal record FogSettingsSpec
{
	[Serialize]
	public Color FogColor { get; init; }

	[Serialize]
	public float FogDensity { get; init; }
}
