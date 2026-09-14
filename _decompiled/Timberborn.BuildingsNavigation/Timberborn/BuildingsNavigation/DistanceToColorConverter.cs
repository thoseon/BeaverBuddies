using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.CoreUI;
using Timberborn.Navigation;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.BuildingsNavigation;

public class DistanceToColorConverter : ILoadableSingleton
{
	private readonly NavigationDistance _navigationDistance;

	private readonly ISpecService _specService;

	private Gradient _distanceGradient;

	public DistanceToColorConverter(NavigationDistance navigationDistance, ISpecService specService)
	{
		_navigationDistance = navigationDistance;
		_specService = specService;
	}

	public void Load()
	{
		GradientColorKey[] colorKeys = _specService.GetSingleSpec<DistanceToColorConverterSpec>().DistanceGradient.Select((GradientPointSpec point) => new GradientColorKey(point.Color, point.Time)).ToArray();
		_distanceGradient = new Gradient
		{
			colorKeys = colorKeys
		};
	}

	public Color DistanceToColor(float distance)
	{
		float time = Mathf.InverseLerp(0f, _navigationDistance.LargeDistrictThreshold, distance);
		return _distanceGradient.Evaluate(time);
	}
}
