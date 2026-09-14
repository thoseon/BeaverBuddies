using Timberborn.UndoSystem;

namespace Timberborn.TerrainUndoSystem;

internal class DeletedTerrainUndoable : IUndoable
{
	private readonly UndoableTerrain _undoableTerrain;

	public DeletedTerrainUndoable(UndoableTerrain undoableTerrain)
	{
		_undoableTerrain = undoableTerrain;
	}

	public void Undo()
	{
		_undoableTerrain.SetTerrain();
	}

	public void Redo()
	{
		_undoableTerrain.UnsetTerrain();
	}
}
