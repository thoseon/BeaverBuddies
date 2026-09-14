namespace Timberborn.TerrainSystem;

public class TerrainHeightChangeEventArgs
{
	public TerrainHeightChange Change { get; }

	public TerrainHeightChangeEventArgs(TerrainHeightChange change)
	{
		Change = change;
	}
}
