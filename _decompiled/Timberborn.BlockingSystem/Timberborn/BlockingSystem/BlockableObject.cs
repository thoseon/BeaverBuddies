using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Navigation;

namespace Timberborn.BlockingSystem;

public class BlockableObject : BaseComponent, IAccessibleValidator
{
	private readonly HashSet<object> _blockers = new HashSet<object>();

	public bool IsUnblocked => _blockers.IsEmpty();

	public bool ValidAccessible => IsUnblocked;

	public event EventHandler ObjectBlocked;

	public event EventHandler ObjectUnblocked;

	public void Block(object blocker)
	{
		if (_blockers.Add(blocker) && _blockers.Count == 1)
		{
			ObjectBlocked?.Invoke(this, EventArgs.Empty);
		}
	}

	public void Unblock(object blocker)
	{
		if (_blockers.Remove(blocker) && _blockers.Count == 0)
		{
			ObjectUnblocked?.Invoke(this, EventArgs.Empty);
		}
	}
}
