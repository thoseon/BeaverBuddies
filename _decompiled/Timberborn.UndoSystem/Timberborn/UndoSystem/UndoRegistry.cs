using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.Common;
using Timberborn.SingletonSystem;

namespace Timberborn.UndoSystem;

internal class UndoRegistry : IUndoRegistry, IUpdatableSingleton
{
	private readonly EventBus _eventBus;

	private readonly ImmutableArray<IUndoPostprocessor> _undoPostprocessors;

	private readonly Stack<IUndoable> _undoStack = new Stack<IUndoable>();

	private readonly Stack<IUndoable> _redoStack = new Stack<IUndoable>();

	private readonly List<IUndoable> _stackToRegister = new List<IUndoable>();

	private bool _activated;

	public bool IsProcessingStack { get; private set; }

	public bool UndoAllowed => true;

	public bool CanUndo => _undoStack.Count > 0;

	public bool CanRedo => _redoStack.Count > 0;

	public UndoRegistry(EventBus eventBus, IEnumerable<IUndoPostprocessor> undoPostprocessors)
	{
		_eventBus = eventBus;
		_undoPostprocessors = undoPostprocessors.ToImmutableArray();
	}

	public void UpdateSingleton()
	{
		if (!_activated)
		{
			_activated = true;
		}
	}

	public void RegisterSingleUndoable(IUndoable undoable)
	{
		if (_activated)
		{
			Asserts.IsFalse(this, IsProcessingStack, "IsProcessingStack");
			AddUndoableToStack(undoable);
		}
	}

	public void RegisterStackedUndoable(IUndoable undoable)
	{
		if (_activated)
		{
			Asserts.IsFalse(this, IsProcessingStack, "IsProcessingStack");
			_stackToRegister.Add(undoable);
		}
	}

	public void CommitStack()
	{
		if (_stackToRegister.Count > 0)
		{
			UndoableStack undoable = new UndoableStack(_stackToRegister);
			AddUndoableToStack(undoable);
			_stackToRegister.Clear();
		}
	}

	public void Undo()
	{
		CommitStack();
		IsProcessingStack = true;
		if (_undoStack.Count > 0)
		{
			IUndoable undoable = _undoStack.Pop();
			undoable.Undo();
			_redoStack.Push(undoable);
		}
		PostprocessUndoables();
		IsProcessingStack = false;
		_eventBus.Post(new UndoStateChangedEvent());
	}

	public void Redo()
	{
		CommitStack();
		IsProcessingStack = true;
		if (_redoStack.Count > 0)
		{
			IUndoable undoable = _redoStack.Pop();
			undoable.Redo();
			_undoStack.Push(undoable);
		}
		PostprocessUndoables();
		IsProcessingStack = false;
		_eventBus.Post(new UndoStateChangedEvent());
	}

	private void AddUndoableToStack(IUndoable undoable)
	{
		_undoStack.Push(undoable);
		_redoStack.Clear();
		_eventBus.Post(new UndoStateChangedEvent());
	}

	private void PostprocessUndoables()
	{
		foreach (IUndoPostprocessor undoPostprocessor in _undoPostprocessors)
		{
			undoPostprocessor.PostprocessUndoables();
		}
	}
}
