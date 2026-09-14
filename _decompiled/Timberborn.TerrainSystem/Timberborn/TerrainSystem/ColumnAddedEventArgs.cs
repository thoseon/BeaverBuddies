namespace Timberborn.TerrainSystem;

public readonly struct ColumnAddedEventArgs
{
	public int Index { get; }

	public int ColumnCount { get; }

	public ColumnAddedEventArgs(int index, int columnCount)
	{
		Index = index;
		ColumnCount = columnCount;
	}
}
