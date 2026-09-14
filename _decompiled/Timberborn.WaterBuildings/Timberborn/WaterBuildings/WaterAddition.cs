namespace Timberborn.WaterBuildings;

public readonly struct WaterAddition
{
	public float CleanWater { get; }

	public float ContaminatedWater { get; }

	public WaterAddition(float cleanWater, float contaminatedWater)
	{
		CleanWater = cleanWater;
		ContaminatedWater = contaminatedWater;
	}
}
