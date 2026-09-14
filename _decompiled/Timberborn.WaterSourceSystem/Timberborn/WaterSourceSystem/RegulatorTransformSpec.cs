using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.WaterSourceSystem;

internal record RegulatorTransformSpec : ComponentSpec
{
	[Serialize]
	public string TransformName { get; init; }

	[Serialize]
	public Vector3 TargetOffset { get; init; }

	[Serialize]
	public Vector3 TargetRotation { get; init; }
}
