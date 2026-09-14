using Timberborn.Common;
using UnityEngine;

namespace Timberborn.DeconstructionSystem;

public class BuildingDeconstructedEvent
{
	public Deconstructible Deconstructible { get; }

	public ReadOnlyList<Vector3Int> Coordinates { get; }

	public BuildingDeconstructedEvent(Deconstructible deconstructible, ReadOnlyList<Vector3Int> coordinates)
	{
		Deconstructible = deconstructible;
		Coordinates = coordinates;
	}
}
