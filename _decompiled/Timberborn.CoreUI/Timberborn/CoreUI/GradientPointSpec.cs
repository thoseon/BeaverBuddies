using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.CoreUI;

public record GradientPointSpec
{
	[Serialize]
	public Color Color { get; init; }

	[Serialize]
	public float Time { get; init; }
}
