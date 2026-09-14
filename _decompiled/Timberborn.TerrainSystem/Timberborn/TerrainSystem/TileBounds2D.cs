namespace Timberborn.TerrainSystem;

public readonly struct TileBounds2D
{
	public int MinX { get; }

	public int MinY { get; }

	public int MaxX { get; }

	public int MaxY { get; }

	public TileBounds2D(int minX, int minY, int maxX, int maxY)
	{
		MinX = minX;
		MinY = minY;
		MaxX = maxX;
		MaxY = maxY;
	}
}
