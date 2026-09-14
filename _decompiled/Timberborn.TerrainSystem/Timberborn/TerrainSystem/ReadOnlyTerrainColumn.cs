using JetBrains.Annotations;

namespace Timberborn.TerrainSystem;

public readonly struct ReadOnlyTerrainColumn
{
	public int Floor { get; }

	public int Ceiling { get; }

	[UsedImplicitly]
	public ReadOnlyTerrainColumn(int floor, int ceiling)
	{
		Floor = floor;
		Ceiling = ceiling;
	}
}
