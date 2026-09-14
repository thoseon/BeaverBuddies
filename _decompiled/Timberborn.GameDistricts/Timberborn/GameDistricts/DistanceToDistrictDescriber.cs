using Timberborn.Localization;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.GameDistricts;

public class DistanceToDistrictDescriber
{
	private static readonly string DistanceLocKey = "Enterable.DistanceToDistrict";

	private static readonly string DistanceLargeLocKey = "Enterable.DistanceToDistrictLarge";

	private readonly NavigationDistance _navigationDistance;

	private readonly ILoc _loc;

	public DistanceToDistrictDescriber(NavigationDistance navigationDistance, ILoc loc)
	{
		_navigationDistance = navigationDistance;
		_loc = loc;
	}

	public string DescribeDistance(float distance)
	{
		int num = Mathf.RoundToInt(distance);
		string text = "<align=\"center\">" + _loc.T(DistanceLocKey, num) + "</align>";
		string text2 = "<color=\"red\">" + _loc.T(DistanceLargeLocKey, num) + "</color>";
		if (!((float)num > _navigationDistance.LargeDistrictThreshold))
		{
			return text;
		}
		return text + "\n" + text2;
	}
}
