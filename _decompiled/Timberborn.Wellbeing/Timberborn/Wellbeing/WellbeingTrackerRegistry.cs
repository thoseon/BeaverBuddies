using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Wellbeing;

internal class WellbeingTrackerRegistry
{
	private readonly HashSet<WellbeingTracker> _wellbeingTrackers = new HashSet<WellbeingTracker>();

	public void Register(WellbeingTracker wellbeingTracker)
	{
		_wellbeingTrackers.Add(wellbeingTracker);
	}

	public void Unregister(WellbeingTracker wellbeingTracker)
	{
		_wellbeingTrackers.Remove(wellbeingTracker);
	}

	public int GetAverageWellbeing()
	{
		if (_wellbeingTrackers.Count > 0)
		{
			float num = 0f;
			foreach (WellbeingTracker wellbeingTracker in _wellbeingTrackers)
			{
				num += (float)wellbeingTracker.Wellbeing;
			}
			return Mathf.RoundToInt(num / (float)_wellbeingTrackers.Count);
		}
		return 0;
	}
}
