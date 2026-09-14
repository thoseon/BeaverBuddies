using Timberborn.UndoSystem;

namespace Timberborn.EntityUndoSystem;

internal class CreatedEntityUndoable : IUndoable
{
	private readonly UndoableEntity _undoableEntity;

	public CreatedEntityUndoable(UndoableEntity undoableEntity)
	{
		_undoableEntity = undoableEntity;
	}

	public void Undo()
	{
		_undoableEntity.InitializeUndoableState();
		_undoableEntity.Delete();
	}

	public void Redo()
	{
		_undoableEntity.Create();
	}
}
