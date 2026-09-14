using Timberborn.BaseComponentSystem;
using Timberborn.WaterBuildings;
using Timberborn.Workshops;

namespace Timberborn.WaterWorkshops;

internal class WaterInputContaminationManufactoryLimiter : BaseComponent, IAwakableComponent, IManufactoryLimiter
{
	private static readonly float EfficiencyThreshold = 0.0001f;

	private WaterInput _waterInput;

	private ManufactoryWaterContaminationConsumer _manufactoryWaterContaminationConsumer;

	public void Awake()
	{
		_waterInput = GetComponent<WaterInput>();
		_manufactoryWaterContaminationConsumer = GetComponent<ManufactoryWaterContaminationConsumer>();
	}

	public float ProductionEfficiency()
	{
		if (!_waterInput.IsUnderwater)
		{
			return 0f;
		}
		float contaminationPercentage = _waterInput.ContaminationPercentage;
		if (contaminationPercentage < EfficiencyThreshold)
		{
			return 0f;
		}
		return contaminationPercentage;
	}

	public float MaxProductionProgressChange(float expectedProductionProgressChange)
	{
		float consumedWaterContamination = _manufactoryWaterContaminationConsumer.ConsumedWaterContamination;
		if (consumedWaterContamination > 0f)
		{
			float neededWater = expectedProductionProgressChange * consumedWaterContamination;
			return _waterInput.DemandContaminatedWaterAmount(neededWater) / consumedWaterContamination;
		}
		return 0f;
	}
}
