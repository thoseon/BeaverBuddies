using Timberborn.BlueprintSystem;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.UnityEngineSpecs;

public record CapsuleColliderSpec
{
	[Serialize]
	public Vector3 Center { get; init; }

	[Serialize]
	public float Radius { get; init; }

	[Serialize]
	public float Height { get; init; }

	[Serialize]
	public Axis Axis { get; init; } = Axis.Y;
}
