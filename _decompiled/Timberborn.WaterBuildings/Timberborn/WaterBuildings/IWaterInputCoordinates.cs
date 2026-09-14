using UnityEngine;

namespace Timberborn.WaterBuildings;

internal interface IWaterInputCoordinates
{
	Vector3Int Coordinates { get; }

	bool IsBlocked { get; }
}
