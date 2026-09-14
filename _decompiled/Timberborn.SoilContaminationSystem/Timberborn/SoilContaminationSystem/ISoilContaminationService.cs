using UnityEngine;

namespace Timberborn.SoilContaminationSystem;

public interface ISoilContaminationService
{
	bool SoilIsContaminated(Vector3Int coordinates);

	float Contamination(int index);
}
