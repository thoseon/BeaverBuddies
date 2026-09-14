using Timberborn.UndoSystem;

namespace Timberborn.EntityUndoSystem;

internal class DeletedEntityUndoable : IUndoable
{
	private readonly UndoableEntity _undoableEntity;

	public DeletedEntityUndoable(UndoableEntity undoableEntity)
	{
		_undoableEntity = undoableEntity;
	}

	public void Undo()
	{
		_undoableEntity.Create();
	}

	public void Redo()
	{
		_undoableEntity.Delete();
	}
}
