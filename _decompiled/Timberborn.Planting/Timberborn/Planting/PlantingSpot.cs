using Timberborn.BlockSystem;
using UnityEngine;

namespace Timberborn.Planting;

public readonly struct PlantingSpot
{
	public Vector3Int Coordinates { get; }

	public string ResourceToPlant { get; }

	public BlockObject PlantingBlocker { get; }

	public PlantingSpot(Vector3Int coordinates, string resourceToPlant, BlockObject plantingBlocker)
	{
		Coordinates = coordinates;
		ResourceToPlant = resourceToPlant;
		PlantingBlocker = plantingBlocker;
	}
}
