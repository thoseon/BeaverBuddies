using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.WaterBuildingsUI;

internal class WaterOutputParticleColors : ILoadableSingleton
{
	private readonly ISpecService _specService;

	public Gradient WaterContaminationParticleGradient { get; private set; }

	public WaterOutputParticleColors(ISpecService specService)
	{
		_specService = specService;
	}

	public void Load()
	{
		GradientColorKey[] colorKeys = _specService.GetSingleSpec<WaterOutputParticleColorsSpec>().WaterContaminationParticleGradient.Select((GradientPointSpec point) => new GradientColorKey(point.Color, point.Time)).ToArray();
		WaterContaminationParticleGradient = new Gradient
		{
			colorKeys = colorKeys
		};
	}
}
