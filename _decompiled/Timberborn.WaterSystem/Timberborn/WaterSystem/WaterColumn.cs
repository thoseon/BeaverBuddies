using System;

namespace Timberborn.WaterSystem;

internal struct WaterColumn(int floor, int ceiling)
{
	public byte Floor = Convert.ToByte(floor);

	public byte Ceiling = Convert.ToByte(ceiling);

	public float WaterDepth = 0f;

	public float OldWaterDepth = 0f;

	public float Contamination = 0f;

	public float Overflow = 0f;

	public void Reset()
	{
		WaterDepth = 0f;
		OldWaterDepth = 0f;
		Contamination = 0f;
		Overflow = 0f;
	}
}
