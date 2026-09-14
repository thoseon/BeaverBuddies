using UnityEngine;

namespace Timberborn.HazardousWeatherSystem;

internal static class HazardousWeatherHelper
{
	public static float GetHandicapMultiplier(int cycle, float handicapMultiplier, float handicapCycles)
	{
		if (handicapCycles > 0f)
		{
			float t = Mathf.Clamp01((float)(cycle - 1) / handicapCycles);
			return Mathf.Lerp(handicapMultiplier, 1f, t);
		}
		return 1f;
	}
}
