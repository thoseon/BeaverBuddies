using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.Buildings;

internal record BuildingAccessibleSpec : ComponentSpec
{
	[Serialize]
	public Vector3 LocalAccess { get; init; }

	[Serialize]
	public bool ForceOneFinalAccess { get; init; }

	public Vector3 CalculateAccessFromLocalAccess(BlockObject blockObject)
	{
		Vector3 coordinates = CoordinateSystem.WorldToGrid(LocalAccess);
		return CoordinateSystem.GridToWorld(blockObject.TransformCoordinates(coordinates));
	}
}
