using UnityEngine;

namespace Timberborn.SoilMoistureSystem;

public interface ISoilMoistureService
{
	bool SoilIsMoist(Vector3Int coordinates);

	float SoilMoisture(int index);
}
