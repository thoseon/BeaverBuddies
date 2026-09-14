using System.Collections.Generic;
using System.Collections.Immutable;

namespace Timberborn.UndoSystem;

internal class UndoableStack : IUndoable
{
	private readonly ImmutableArray<IUndoable> _stack;

	public UndoableStack(List<IUndoable> stack)
	{
		_stack = stack.ToImmutableArray();
	}

	public void Undo()
	{
		for (int num = _stack.Length - 1; num >= 0; num--)
		{
			_stack[num].Undo();
		}
	}

	public void Redo()
	{
		for (int i = 0; i < _stack.Length; i++)
		{
			_stack[i].Redo();
		}
	}
}
