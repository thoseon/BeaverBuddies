namespace Timberborn.WaterSystem;

public readonly struct WaterAmountChange
{
	public float CleanWaterChange { get; }

	public float ContaminatedWaterChange { get; }

	public WaterAmountChange(float cleanWaterChange, float contaminatedWaterChange)
	{
		CleanWaterChange = cleanWaterChange;
		ContaminatedWaterChange = contaminatedWaterChange;
	}
}
