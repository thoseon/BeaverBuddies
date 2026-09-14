using Timberborn.Common;
using UnityEngine;

namespace Timberborn.WaterBuildings;

internal class PreviewBlockingCoordinatesChangedEvent
{
	public ReadOnlyList<Vector3Int> ChangedCoordinates { get; }

	public PreviewBlockingCoordinatesChangedEvent(ReadOnlyList<Vector3Int> changedCoordinates)
	{
		ChangedCoordinates = changedCoordinates;
	}
}
