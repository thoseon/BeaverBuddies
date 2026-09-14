using JetBrains.Annotations;

namespace Timberborn.WaterSystem;

public readonly struct ReadOnlyWaterColumn
{
	[UsedImplicitly]
	public byte Floor { get; }

	[UsedImplicitly]
	public byte Ceiling { get; }

	[UsedImplicitly]
	public float WaterDepth { get; }

	[UsedImplicitly]
	public float OldWaterDepth { get; }

	[UsedImplicitly]
	public float Contamination { get; }

	[UsedImplicitly]
	public float Overflow { get; }
}
