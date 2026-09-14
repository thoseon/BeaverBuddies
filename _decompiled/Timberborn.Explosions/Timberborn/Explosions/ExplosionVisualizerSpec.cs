using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.Explosions;

internal record ExplosionVisualizerSpec : ComponentSpec
{
	[Serialize]
	public Color ObjectHighlightColor { get; init; }
}
