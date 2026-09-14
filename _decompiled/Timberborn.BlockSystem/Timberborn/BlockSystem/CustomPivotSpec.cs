using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.BlockSystem;

public record CustomPivotSpec
{
	[Serialize]
	public bool HasCustomPivot { get; init; }

	[Serialize]
	public Vector3 Coordinates { get; init; }
}
