using System;
using Timberborn.UndoSystem;

namespace Timberborn.GameScene;

public class DummyUndoRegistry : IUndoRegistry
{
	public bool UndoAllowed => false;

	public bool IsProcessingStack => false;

	public bool CanUndo => false;

	public bool CanRedo => false;

	public void RegisterSingleUndoable(IUndoable undoable)
	{
	}

	public void RegisterStackedUndoable(IUndoable undoable)
	{
	}

	public void CommitStack()
	{
	}

	public void Undo()
	{
		throw new NotSupportedException();
	}

	public void Redo()
	{
		throw new NotSupportedException();
	}
}
