namespace Timberborn.UndoSystem;

public interface IUndoRegistry
{
	bool UndoAllowed { get; }

	bool IsProcessingStack { get; }

	bool CanUndo { get; }

	bool CanRedo { get; }

	void RegisterSingleUndoable(IUndoable undoable);

	void RegisterStackedUndoable(IUndoable undoable);

	void CommitStack();

	void Undo();

	void Redo();
}
