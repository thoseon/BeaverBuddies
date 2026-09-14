namespace Timberborn.ScreenSystem;

public readonly struct ScreenResolution
{
	public int Width { get; }

	public int Height { get; }

	public ScreenResolution(int width, int height)
	{
		Width = width;
		Height = height;
	}
}
