using Timberborn.UndoSystem;

namespace Timberborn.TerrainUndoSystem;

internal class CreatedTerrainUndoable : IUndoable
{
	private readonly UndoableTerrain _undoableTerrain;

	public CreatedTerrainUndoable(UndoableTerrain undoableTerrain)
	{
		_undoableTerrain = undoableTerrain;
	}

	public void Undo()
	{
		_undoableTerrain.UnsetTerrain();
	}

	public void Redo()
	{
		_undoableTerrain.SetTerrain();
	}
}
