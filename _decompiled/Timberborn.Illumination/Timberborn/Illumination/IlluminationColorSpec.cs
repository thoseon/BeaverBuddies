using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.Illumination;

internal record IlluminationColorSpec : ComponentSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public Color Color { get; init; }
}
