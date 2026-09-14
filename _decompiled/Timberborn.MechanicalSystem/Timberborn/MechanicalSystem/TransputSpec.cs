using Timberborn.BlueprintSystem;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.MechanicalSystem;

public record TransputSpec
{
	[Serialize]
	public Vector3Int Coordinates { get; init; }

	[Serialize]
	public Directions3D Directions { get; init; }

	[Serialize]
	public bool ReverseRotation { get; init; }
}
