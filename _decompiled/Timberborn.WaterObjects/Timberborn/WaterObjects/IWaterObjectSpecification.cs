using UnityEngine;

namespace Timberborn.WaterObjects;

public interface IWaterObjectSpecification
{
	Vector3Int WaterCoordinates { get; }
}
