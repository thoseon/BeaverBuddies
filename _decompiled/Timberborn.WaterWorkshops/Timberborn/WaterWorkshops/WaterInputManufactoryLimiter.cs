using Timberborn.BaseComponentSystem;
using Timberborn.WaterBuildings;
using Timberborn.Workshops;

namespace Timberborn.WaterWorkshops;

internal class WaterInputManufactoryLimiter : BaseComponent, IAwakableComponent, IManufactoryLimiter
{
	private static readonly float EfficiencyThreshold = 0.0001f;

	private WaterInput _waterInput;

	private ManufactoryWaterConsumer _manufactoryWaterConsumer;

	public void Awake()
	{
		_waterInput = GetComponent<WaterInput>();
		_manufactoryWaterConsumer = GetComponent<ManufactoryWaterConsumer>();
	}

	public float ProductionEfficiency()
	{
		if (!_waterInput.IsUnderwater)
		{
			return 0f;
		}
		float num = 1f - _waterInput.ContaminationPercentage;
		if (num < EfficiencyThreshold)
		{
			return 0f;
		}
		return num;
	}

	public float MaxProductionProgressChange(float expectedProductionProgressChange)
	{
		float consumedWater = _manufactoryWaterConsumer.ConsumedWater;
		if (consumedWater > 0f)
		{
			float neededWater = expectedProductionProgressChange * consumedWater;
			return _waterInput.DemandCleanWaterAmount(neededWater) / consumedWater;
		}
		return 0f;
	}
}
