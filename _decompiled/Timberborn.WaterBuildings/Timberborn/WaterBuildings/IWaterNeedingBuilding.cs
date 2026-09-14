using System;
using UnityEngine;

namespace Timberborn.WaterBuildings;

public interface IWaterNeedingBuilding
{
	Vector3Int WaterCoordinatesTransformed { get; }

	event EventHandler StartedNeedingWater;

	event EventHandler StoppedNeedingWater;
}
