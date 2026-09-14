using UnityEngine;

namespace Timberborn.Planting;

public class PlantingCoordinatesSetEvent
{
	public Vector3Int Coordinates { get; }

	public string Resource { get; }

	public PlantingCoordinatesSetEvent(Vector3Int coordinates, string resource)
	{
		Coordinates = coordinates;
		Resource = resource;
	}
}
