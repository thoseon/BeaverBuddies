using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.WindSystem;

internal record WindRotatorSpec
{
	[Serialize]
	public string TransformName { get; init; }

	[Serialize]
	public Vector3 RotationAxis { get; init; }

	[Serialize]
	public float RotationSpeed { get; init; }
}
