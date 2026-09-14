using UnityEngine;

namespace Timberborn.Common;

public static class Numbers
{
	public static float RoundToPrecision(float value, float precision)
	{
		return Mathf.Round(value / precision) * precision;
	}
}
