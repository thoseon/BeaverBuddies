using UnityEngine;

namespace Timberborn.TutorialSystemUI;

internal static class HighlightTimer
{
	public static bool IsTimeForSteadyHighlight()
	{
		return IsTimeForHighlight(0.5f);
	}

	public static bool IsTimeForPulsingHighlight()
	{
		if (IsTimeForHighlight(3f))
		{
			return IsTimeForHighlight(0.5f);
		}
		return false;
	}

	private static bool IsTimeForHighlight(float highlightDuration)
	{
		return Time.unscaledTime % (highlightDuration * 2f) > highlightDuration;
	}
}
