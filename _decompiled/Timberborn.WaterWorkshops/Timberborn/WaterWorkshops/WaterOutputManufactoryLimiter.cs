using Timberborn.BaseComponentSystem;
using Timberborn.WaterBuildings;
using Timberborn.Workshops;

namespace Timberborn.WaterWorkshops;

internal class WaterOutputManufactoryLimiter : BaseComponent, IAwakableComponent, IManufactoryLimiter
{
	private static readonly float DepthOffset = 0.01f;

	private WaterOutput _waterOutput;

	private Workshop _workshop;

	public void Awake()
	{
		_waterOutput = GetComponent<WaterOutput>();
		_workshop = GetComponent<Workshop>();
	}

	public float ProductionEfficiency()
	{
		float num = (_workshop.CurrentlyWorking ? DepthOffset : (0f - DepthOffset));
		if (!(_waterOutput.AvailableSpace + num > 0f))
		{
			return 0f;
		}
		return 1f;
	}

	public float MaxProductionProgressChange(float expectedProductionProgressChange)
	{
		return 1f;
	}
}
