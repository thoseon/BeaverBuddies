using System;

namespace Timberborn.ToolSystem;

public interface IToolLocker
{
	bool ShouldLock(ITool tool);

	void TryToUnlock(ITool tool, Action successCallback, Action failCallback);
}
