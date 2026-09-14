using Timberborn.BlueprintSystem;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.PathSystem;

internal record DrivewayModelSpec
{
	[Serialize]
	public Driveway Driveway { get; init; }

	[Serialize]
	public bool HasCustomCoordinates { get; init; }

	[Serialize]
	public Vector3Int CustomCoordinates { get; init; }

	[Serialize]
	public Direction2D CustomDirection { get; init; }

	[Serialize]
	public DrivewayMode DrivewayMode { get; init; }
}
