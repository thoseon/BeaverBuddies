using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.WaterSourceSystem;

public class WaterStrengthService : ILoadableSingleton
{
	private readonly ISpecService _specService;

	public float MaxWaterSourceStrength { get; private set; }

	public float MaxWaterSourceChangePerSecond { get; private set; }

	public float MinWaterSourceChangeScaler { get; private set; }

	public WaterStrengthService(ISpecService specService)
	{
		_specService = specService;
	}

	public void Load()
	{
		WaterStrengthSpec singleSpec = _specService.GetSingleSpec<WaterStrengthSpec>();
		MaxWaterSourceStrength = singleSpec.MaxWaterSourceStrength;
		MaxWaterSourceChangePerSecond = singleSpec.MaxWaterSourceChangePerSecond;
		MinWaterSourceChangeScaler = singleSpec.MinWaterSourceChangeScaler;
	}
}
