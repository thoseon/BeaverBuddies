using UnityEngine;

namespace Timberborn.ConstructionSites;

public class ConstructionSiteBuildTimeCalculator
{
	private static readonly int DefaultTimeInHours = 1;

	private static readonly float MaterialToHourRatio = 20f;

	public float GetConstructionTimeInHours(ConstructionSite constructionSite)
	{
		ConstructionSiteBuildTimeSpec component = constructionSite.GetComponent<ConstructionSiteBuildTimeSpec>();
		if ((object)component != null)
		{
			return component.ConstructionTimeInHours;
		}
		int capacity = constructionSite.Inventory.Capacity;
		return (capacity == 0) ? DefaultTimeInHours : Mathf.CeilToInt((float)capacity / MaterialToHourRatio);
	}
}
