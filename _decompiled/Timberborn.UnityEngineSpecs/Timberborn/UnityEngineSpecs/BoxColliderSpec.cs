using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.UnityEngineSpecs;

public record BoxColliderSpec
{
	[Serialize]
	public Vector3 Center { get; init; }

	[Serialize]
	public Vector3 Size { get; init; }
}
