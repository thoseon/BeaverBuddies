using UnityEngine;

namespace Timberborn.Planting;

public class PlantingCoordinatesUnsetEvent
{
	public Vector3Int Coordinates { get; }

	public string Resource { get; }

	public PlantingCoordinatesUnsetEvent(Vector3Int coordinates, string resource)
	{
		Coordinates = coordinates;
		Resource = resource;
	}
}
