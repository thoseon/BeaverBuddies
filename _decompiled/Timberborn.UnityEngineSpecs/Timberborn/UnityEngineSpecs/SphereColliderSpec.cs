using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.UnityEngineSpecs;

public record SphereColliderSpec
{
	[Serialize]
	public Vector3 Center { get; init; }

	[Serialize]
	public float Radius { get; init; }
}
