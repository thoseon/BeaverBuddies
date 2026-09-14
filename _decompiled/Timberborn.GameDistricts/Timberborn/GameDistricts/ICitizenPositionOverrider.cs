using UnityEngine;

namespace Timberborn.GameDistricts;

public interface ICitizenPositionOverrider
{
	bool TryGetOverridenPosition(out Vector3 position);
}
