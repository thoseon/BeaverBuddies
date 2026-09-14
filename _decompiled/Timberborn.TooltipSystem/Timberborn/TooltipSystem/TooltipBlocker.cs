using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.TooltipSystem;

public class TooltipBlocker
{
	private readonly HashSet<object> _blockers = new HashSet<object>();

	public bool IsUnblocked => _blockers.IsEmpty();

	public void AddBlocker(object blocker)
	{
		_blockers.Add(blocker);
	}

	public void RemoveBlocker(object blocker)
	{
		_blockers.Remove(blocker);
	}
}
