namespace Timberborn.TerrainSystem;

public readonly struct ColumnRemovedEventArgs
{
	public int Index { get; }

	public int ColumnCount { get; }

	public ColumnRemovedEventArgs(int index, int columnCount)
	{
		Index = index;
		ColumnCount = columnCount;
	}
}
