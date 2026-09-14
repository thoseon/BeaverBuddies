namespace Timberborn.WaterSystem;

internal class WaterDepthSetter
{
	private readonly WaterOverflowCalculator _waterOverflowCalculator;

	public WaterDepthSetter(WaterOverflowCalculator waterOverflowCalculator)
	{
		_waterOverflowCalculator = waterOverflowCalculator;
	}

	public void SetWaterDepth(float waterDepthChange, ref WaterColumn waterColumn)
	{
		float num = waterColumn.WaterDepth + waterColumn.Overflow + waterDepthChange;
		int num2 = waterColumn.Ceiling - waterColumn.Floor;
		waterColumn.OldWaterDepth = waterColumn.WaterDepth;
		if (num < 0f)
		{
			waterColumn.WaterDepth = 0f;
			waterColumn.Overflow = 0f;
		}
		else if (num > (float)num2)
		{
			waterColumn.WaterDepth = num2;
			waterColumn.Overflow = _waterOverflowCalculator.ClampOverflow(num, num2, waterColumn.Ceiling);
		}
		else
		{
			waterColumn.WaterDepth = num;
			waterColumn.Overflow = 0f;
		}
	}
}
