using UnityEngine;

namespace Timberborn.WaterSystem;

public interface IWaterRemovalService
{
	WaterAmountChange GetWaterChangeUnsafe(Vector3Int coordinates);
}
