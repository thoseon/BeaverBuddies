namespace Timberborn.NaturalResourcesMoisture;

public readonly struct WaterNeedsUnmetEventArgs
{
	public bool Flooded { get; }

	public WaterNeedsUnmetEventArgs(bool flooded)
	{
		Flooded = flooded;
	}
}
