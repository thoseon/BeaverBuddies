using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.UnityEngineSpecs;

public record TransformSpec : ComponentSpec
{
	[Serialize]
	public Vector3 Position { get; init; }

	[Serialize]
	public Vector3 Rotation { get; init; }

	[Serialize]
	public Vector3 Scale { get; init; } = Vector3.one;
}
