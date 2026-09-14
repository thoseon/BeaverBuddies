using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.SingletonSystem;

namespace Timberborn.ToolSystem;

public class ToolUnlockingService
{
	private readonly EventBus _eventBus;

	private readonly ImmutableArray<IToolLocker> _toolLockers;

	private readonly Dictionary<ITool, IToolLocker> _activeLockers = new Dictionary<ITool, IToolLocker>();

	public ToolUnlockingService(EventBus eventBus, IEnumerable<IToolLocker> toolLockers)
	{
		_eventBus = eventBus;
		_toolLockers = toolLockers.ToImmutableArray();
	}

	public bool IsLocked(ITool tool)
	{
		return _activeLockers.ContainsKey(tool);
	}

	public void LockIfNeeded(ITool tool)
	{
		IToolLocker toolLocker = _toolLockers.SingleOrDefault((IToolLocker locker) => locker.ShouldLock(tool));
		if (toolLocker != null)
		{
			_activeLockers[tool] = toolLocker;
			_eventBus.Post(new ToolLockedEvent(tool));
		}
	}

	public void Unlock(ITool tool)
	{
		if (IsLocked(tool))
		{
			UnlockInternal(tool, delegate
			{
			});
			return;
		}
		throw new InvalidOperationException($"Tool {tool} is not locked, cannot unlock it.");
	}

	public void TryToUnlock(ITool tool)
	{
		TryToUnlock(tool, delegate
		{
		}, delegate
		{
		});
	}

	public void TryToUnlock(ITool tool, Action successCallback, Action failCallback)
	{
		if (_activeLockers.TryGetValue(tool, out var value))
		{
			value.TryToUnlock(tool, delegate
			{
				UnlockInternal(tool, successCallback);
			}, failCallback);
			return;
		}
		throw new InvalidOperationException($"Tool {tool} is not locked, cannot unlock it.");
	}

	private void UnlockInternal(ITool tool, Action successCallback)
	{
		if (_activeLockers.Remove(tool))
		{
			_eventBus.Post(new ToolUnlockedEvent(tool));
			successCallback();
		}
	}
}
